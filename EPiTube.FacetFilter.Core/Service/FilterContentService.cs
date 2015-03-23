using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;
using EPiTube.facetFilter.Core;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.Filters;
using EPiTube.FacetFilter.Core.FilterSettings;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Service
{
    [ServiceConfiguration(typeof(FilterContentService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class FilterContentService
    {
        private class FilterContentModelType
        {
            public IFilterContent Filter { get; set; }
            public Type ContentType { get; set; }
            public Type QueryContentType { get; set; }
            public bool FacetAdded { get; set; }
            public bool HasGenericArgument { get; set; }
            public FacetFilterSetting Setting { get; set; }
        }

        private const int MaxItems = 500;
        private const string SearchMethodName = "Search";

        private IClient _client;
        private readonly Lazy<IEnumerable<FilterContentModelType>> _filterContentsWithGenericTypes;
        private readonly FilterConfiguration _filterConfiguration;
        private readonly CheckedOptionsService _filterModelFactory;
        private readonly IContentRepository _contentRepository;
        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;
        private readonly SearchSortingService _searchSorter;

        public FilterContentService(
            FilterConfiguration filterConfiguration,
            CheckedOptionsService filterModelFactory,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache,
            SearchSortingService searchSorter)
        {
            _filterConfiguration = filterConfiguration;
            _filterModelFactory = filterModelFactory;
            _contentRepository = contentRepository;
            _synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
            _searchSorter = searchSorter;

            _filterContentsWithGenericTypes = new Lazy<IEnumerable<FilterContentModelType>>(FilterContentsWithGenericTypesValueFactory, false);
        }

        public IClient Client { get { return _client ?? (_client = SearchClient.Instance); } }

        public IEnumerable<FilterContentWithOptions> GetFilters(ContentQueryParameters parameters)
        {
            var result = GetFilteredChildren(parameters, false, true);
            return result.Filters.OrderBy(x => x.FilterContent.SortOrder);
        }

        public FacetContentCollection GetFilteredChildren(ContentQueryParameters parameters, bool includeMainSearch, bool includefacets)
        {
            var filterModelString = parameters.AllParameters["filterModel"];
            var productGroupedString = parameters.AllParameters["productGrouped"];

            var content = _contentRepository.Get<IContent>(parameters.ReferenceId);
            bool productGrouped;
            Boolean.TryParse(productGroupedString, out productGrouped);

            var filter = _filterModelFactory.CreateFilterModel(filterModelString);

            return GetFilteredChildren(
                    content,
                    filter,
                    productGrouped,
                    parameters.SortColumns != null ? parameters.SortColumns.FirstOrDefault() : new SortColumn(),
                    parameters.Range, includeMainSearch, includefacets);
        }

        public FacetContentCollection GetFilteredChildren(
            IContent content,
            FilterModel filter,
            bool productGrouped,
            SortColumn sortColumn,
            ItemRange range,
            bool includeMainSearch,
            bool includefacets)
        {
            var searchType = typeof(CatalogContentBase); // make this selectable using Conventions api
            var filters = new Dictionary<string, IEnumerable<object>>();
            if (filter != null && filter.CheckedItems != null)
            {
                filters = filter.CheckedItems.Where(x => x.Value != null)
                    .ToDictionary(k => k.Key, v => v.Value.Select(x => x));

                var receivedSearchType = GetSearchType(filter);
                if (receivedSearchType != null)
                {
                    searchType = receivedSearchType;
                }
            }

            var includeProductVariationRelations = productGrouped && !(content is ProductContent);
            var supportedFilters = SupportedFilters(searchType).ToList();
            var query = CreateSearchQuery(searchType);

            var subQueries = new Dictionary<FilterContentModelType, ISearch>();
            if (includefacets)
            {
                var possiblefacetQueries = _filterContentsWithGenericTypes.Value.Where(x =>
                    x.ContentType.IsAssignableFrom(searchType) ||
                    searchType.IsAssignableFrom(x.ContentType)).ToList();

                AddSubqueries(possiblefacetQueries, subQueries);
            }

            var startIndex = range.Start ?? 0;
            var endIndex = includeProductVariationRelations ? MaxItems : range.End ?? MaxItems;

            var cacheKey = content.ContentLink.ToString();
            if (!String.IsNullOrEmpty(sortColumn.ColumnName))
            {
                cacheKey += sortColumn.ColumnName + sortColumn.SortDescending;
            }

            if (!includeProductVariationRelations)
            {
                cacheKey += startIndex + endIndex;
            }

            AddFilters(content, supportedFilters, filters, query, ref cacheKey);
            AddFiltersToSubQueries(content, subQueries, supportedFilters, filters);

            query = _searchSorter.Sort(sortColumn, query);

            var cachedItems = GetCachedContent(cacheKey);
            if (cachedItems != null)
            {
                range.Total = cachedItems.Item2;
                return cachedItems.Item1;
            }

            var properties = _contentRepository.GetDefault<FacetContent>(content.ContentLink).Property;

            var contentList = new FacetContentCollection();
            var linkedProductLinks = new List<ContentReference>();

            var total = AddFilteredChildren(query, subQueries, contentList, linkedProductLinks,
                properties, includeMainSearch, includeProductVariationRelations, startIndex, endIndex);

            range.Total = includeProductVariationRelations ? contentList.Count : total;
            
            Cache(cacheKey, new Tuple<FacetContentCollection, int>(contentList, total));
            return contentList;
        }

        private static void AddFilters(IContent content, IEnumerable<FilterContentModelType> supportedFilters, Dictionary<string, IEnumerable<object>> filters, ISearch query, ref string cacheKey)
        {
            foreach (var supportedFilter in supportedFilters)
            {
                var filterValues = (filters.ContainsKey(supportedFilter.Filter.Name)
                    ? filters[supportedFilter.Filter.Name]
                    : Enumerable.Empty<object>()).ToArray();

                query = supportedFilter.Filter.Filter(content, query, filterValues);

                cacheKey += String.Join(";", filterValues);
            }
        }

        private static void AddFiltersToSubQueries(IContent content, Dictionary<FilterContentModelType, ISearch> subQueries, List<FilterContentModelType> supportedFilters, Dictionary<string, IEnumerable<object>> filters)
        {
            if (filters == null) throw new ArgumentNullException("filters");
            var subQueryFilterContentModelTypes = subQueries.Keys.ToList();
            foreach (var subQueryKey in subQueryFilterContentModelTypes)
            {
                foreach (
                    var supportedFilter in
                        supportedFilters.Where(x => x.ContentType.IsAssignableFrom(subQueryKey.QueryContentType)))
                {
                    if (!subQueryKey.FacetAdded && supportedFilter.HasGenericArgument &&
                        (supportedFilter.Filter.Name == subQueryKey.Filter.Name ||
                         !subQueryKey.ContentType.IsAssignableFrom(supportedFilter.ContentType)))
                    {
                        subQueries[subQueryKey] = subQueryKey.Filter.AddfacetToQuery(subQueries[subQueryKey]);
                        subQueryKey.FacetAdded = true;

                        if (supportedFilter.Filter.Name == subQueryKey.Filter.Name)
                        {
                            continue;
                        }
                    }

                    var filterValues = (filters.ContainsKey(supportedFilter.Filter.Name)
                        ? filters[supportedFilter.Filter.Name]
                        : Enumerable.Empty<object>()).ToArray();

                    subQueries[subQueryKey] = supportedFilter.Filter.Filter(content, subQueries[subQueryKey], filterValues);
                }

                if (!subQueryKey.FacetAdded)
                {
                    subQueries[subQueryKey] = subQueryKey.Filter.AddfacetToQuery(subQueries[subQueryKey]);
                    subQueryKey.FacetAdded = true;
                }
            }
        }

        private void AddSubqueries(List<FilterContentModelType> possiblefacetQueries, Dictionary<FilterContentModelType, ISearch> subQueries)
        {
            foreach (var filterContentModelType in possiblefacetQueries)
            {
                if (subQueries.ContainsKey(filterContentModelType))
                {
                    subQueries[filterContentModelType] =
                        filterContentModelType.Filter.AddfacetToQuery(subQueries[filterContentModelType]);
                    continue;
                }

                filterContentModelType.QueryContentType = filterContentModelType.ContentType;
                foreach (var otherFilterContentModelType in possiblefacetQueries)
                {
                    if (filterContentModelType.QueryContentType.IsAssignableFrom(otherFilterContentModelType.ContentType))
                    {
                        filterContentModelType.QueryContentType = otherFilterContentModelType.ContentType;
                    }
                }

                var subQuery = CreateSearchQuery(filterContentModelType.QueryContentType);
                subQueries.Add(filterContentModelType, subQuery);
            }
        }


        private Tuple<FacetContentCollection, int> GetCachedContent(string cacheKey)
        {
            return _synchronizedObjectInstanceCache.Get(cacheKey) as Tuple<FacetContentCollection, int>;
        }

        private void Cache(string cacheKey, Tuple<FacetContentCollection, int> result)
        {
            _synchronizedObjectInstanceCache.Insert(
                cacheKey, 
                result, 
                new CacheEvictionPolicy(null, null, new[]
                {
                    DataFactoryCache.RootKeyName, 
                    "EP:CatalogKeyPricesMasterCacheKey", 
                    "Mediachase.Commerce.InventoryService.Storage$MASTER"
                }, 
                new TimeSpan(1, 0, 0), 
                CacheTimeoutType.Sliding));
        }

        private int AddFilteredChildren(
            ISearch query,
            Dictionary<FilterContentModelType, ISearch> subQueries,
            FacetContentCollection contentList,
            List<ContentReference> linkedProductLinks,
            PropertyDataCollection properties,
            bool includeMainSearch,
            bool includeProductVariationRelations, 
            int startIndex, 
            int take)
        {
            try
            {
                var total = 0;

                if (subQueries.Any())
                {
                    AddFilterResult(subQueries, contentList);
                }

                if (includeMainSearch)
                {
                    var queryResult = GetSearchResult(query, properties, startIndex, take);

                    total = queryResult.TotalMatching;
                    foreach (var resultItem in queryResult)
                    {
                        // When we ask for relations, add product links to linkedProductList if any exists, and do not add a model for the content if it has product links.
                        if (includeProductVariationRelations)
                        {
                            if (resultItem.ProductLinks != null && resultItem.ProductLinks.Any())
                            {
                                foreach (var productLink in resultItem.ProductLinks)
                                {
                                    if (!linkedProductLinks.Contains(productLink))
                                    {
                                        linkedProductLinks.Add(productLink);
                                    }
                                }

                                continue;
                            }
                        }

                        if (!contentList.Any(x => x.ContentLink.CompareToIgnoreWorkID(resultItem.ContentLink)))
                        {
                            contentList.Add(resultItem);
                        }
                    }

                    if (includeProductVariationRelations)
                    {
                        // get parent products in the query result
                        var addedContentLinks = contentList.Select(x => x.ContentLink);
                        var links = addedContentLinks;
                        var notAddedLinkedProducts = linkedProductLinks.Where(x => !links.Contains(x));
                        foreach (var linkedProductLink in notAddedLinkedProducts)
                        {
                            var resultItem =
                                queryResult.FirstOrDefault(x => x.ContentLink.CompareToIgnoreWorkID(linkedProductLink));
                            if (resultItem != null)
                            {
                                contentList.Add(resultItem);
                            }
                        }

                        addedContentLinks = contentList.Select(x => x.ContentLink);
                        notAddedLinkedProducts = linkedProductLinks.Where(x => !addedContentLinks.Contains(x)).ToArray();
                        if (notAddedLinkedProducts.Any())
                        {
                            var filterBuilder = new FilterBuilder<ProductContent>(SearchClient.Instance);
                            filterBuilder = notAddedLinkedProducts.Aggregate(filterBuilder,
                                (current, reference) => current.Or(x => x.ContentLink.Match(reference)));

                            var productQuery = SearchClient.Instance.Search<ProductContent>().Filter(filterBuilder);
                            total += AddFilteredChildren(productQuery, subQueries, contentList,
                                linkedProductLinks, properties, true, false, 0,
                                MaxItems);
                        }
                    }
                }

                return total;   
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        private static SearchResults<FacetContent> GetSearchResult(ISearch query, PropertyDataCollection properties, int startIndex, int take)
        {
            SearchResults<FacetContent> queryResult;
            var catalogContentSearch = query as ITypeSearch<CatalogContentBase>;
            if (catalogContentSearch != null)
            {
                queryResult = GetSearchResults(catalogContentSearch, properties, startIndex, take + 2);
            }
            else
            {
                var otherSupportedModel = query as ITypeSearch<IFacetContent>;
                if (otherSupportedModel == null)
                {
                    throw new NotSupportedException(
                        "The type needs to inherit from CatalogContentBase, or implement IEPifacetModel");
                }

                queryResult = GetSearchResults(otherSupportedModel, properties, startIndex, take + 2);
            }
            return queryResult;
        }

        private void AddFilterResult(Dictionary<FilterContentModelType, ISearch> subQueries, FacetContentCollection contentList)
        {
            var multSearch = SearchClient.Instance.MultiSearch<FacetContent>();
            var filterListInResultOrder = new Dictionary<IFilterContent, FacetFilterSetting>();
            foreach (var subQuery in subQueries.OrderBy(x => x.Key.Filter.Name))
            {
                var typeSearch = subQuery.Value as ITypeSearch<object>;
                multSearch.Searches.Add(typeSearch.Select(x => new FacetContent()).Take(0));

                foreach (
                    var filterContentModelType in
                        _filterContentsWithGenericTypes.Value.Where(x => x.Filter.Name == subQuery.Key.Filter.Name))
                {
                    if (contentList.Filters.Select(x => x.FilterContent.Name).Contains(filterContentModelType.Filter.Name))
                    {
                        continue;
                    }

                    filterListInResultOrder.Add(filterContentModelType.Filter, filterContentModelType.Setting);
                }
            }

            var filterListInResultOrderKeys = filterListInResultOrder.Keys.ToArray();
            var multiResult = multSearch.GetResult().ToList();
            for (var i = 0; i < multiResult.Count; i++)
            {
                var option = new FilterContentWithOptions()
                {
                    FilterContent = filterListInResultOrderKeys[i],
                    FilterOptions = filterListInResultOrderKeys[i].GetFilterOptions(multiResult[i]).ToArray(),
                };

                var settings = filterListInResultOrder[filterListInResultOrderKeys[i]];
                if (settings != null)
                {
                    option.Settings = settings;
                }

                contentList.AddFilter(option);
            }
        }

        private static SearchResults<FacetContent> GetSearchResults(ITypeSearch<CatalogContentBase> query, PropertyDataCollection properties, int skip, int take)
        {
            return query
                .Select(x => new FacetContent
                {
                    PropertyCollection = properties,
                    Name = x.Name,
                    ContentGuid = x.ContentGuid,
                    ContentLink = x.ContentLink,
                    IsDeleted = x.IsDeleted,
                    VariationLinks = x.VariationLinks(),
                    ParentLink = x.ParentLink,
                    StartPublish = x.StartPublish,
                    StopPublish = x.StopPublish,
                    Code = x.Code(),
                    DefaultPrice = x.DefaultPrice(),
                    ContentTypeID = x.ContentTypeID,
                    ApplicationId = x.ApplicationId,
                    MetaClassId = x.MetaClassId(),
                    ProductLinks = x.ProductLinks(),
                    NodeLinks = x.NodeLinks(),
                    ThumbnailPath = x.ThumbnailPath(),
                    DefaultCurrency = x.DefaultCurrency(),
                    WeightBase = x.WeightBase(),
                    LengthBase = x.LengthBase(),
                    Prices = x.Prices(),
                    Inventories = x.Inventories()
                })
                .Skip(skip)
                .Take(take)
                .GetResult();
        }

        private static SearchResults<FacetContent> GetSearchResults(ITypeSearch<IFacetContent> query, PropertyDataCollection properties, int skip, int take)
        {
            return query
                .Select(x => new FacetContent
                {
                    PropertyCollection = properties,
                    Name = x.Name,
                    ContentGuid = x.ContentGuid,
                    ContentLink = x.ContentLink,
                    IsDeleted = x.IsDeleted,
                    VariationLinks = x.VariationLinks,
                    ParentLink = x.ParentLink,
                    StartPublish = x.StartPublish,
                    StopPublish = x.StopPublish,
                    Code = x.Code,
                    DefaultPrice = x.DefaultPrice,
                    ContentTypeID = x.ContentTypeID,
                    ApplicationId = x.ApplicationId,
                    MetaClassId = x.MetaClassId,
                    ProductLinks = x.ProductLinks,
                    NodeLinks = x.NodeLinks,
                    ThumbnailPath = x.ThumbnailPath,
                    DefaultCurrency = x.DefaultCurrency,
                    WeightBase = x.WeightBase,
                    LengthBase = x.LengthBase,
                    Prices = x.Prices,
                    Inventories = x.Inventories
                })
                .Skip(skip)
                .Take(take)
                .GetResult();
        }

        public Type GetSearchType(FilterModel filterModel)
        {
            Type selectedType = null;
            foreach (var filter in filterModel.CheckedItems)
            {
                if (!filter.Value.Any())
                {
                    continue;
                }

                var filterContentModelType = _filterContentsWithGenericTypes.Value
                    .SingleOrDefault(x => x.Filter.Name == filter.Key);

                if (filterContentModelType == null)
                {
                    continue;
                }

                if(selectedType == null || selectedType.IsAssignableFrom(filterContentModelType.ContentType))
                {
                    selectedType = filterContentModelType.ContentType;
                }
            }

            return selectedType;
        }

        private ISearch CreateSearchQuery(Type contentType)
        {
            // Consider another way of creating an instance of the generic search. Invoke is pretty slow.
            var method = typeof(Client).GetMethod(SearchMethodName, Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(contentType);
            return genericMethod.Invoke(Client, null) as ISearch;
        }

        private IEnumerable<FilterContentModelType> SupportedFilters(Type queryType)
        {
            var supportedTypes = _filterContentsWithGenericTypes.Value.Where(x => x.ContentType.IsAssignableFrom(queryType)).ToArray(); //.OrderBy(x => x.Filter.Name)
            for (var i = 0; i < supportedTypes.Length; i++)
            {
                for (var j = i; j < supportedTypes.Length; j++)
                {
                    if (supportedTypes[i].HasGenericArgument && (!supportedTypes[j].HasGenericArgument || supportedTypes[i].ContentType.IsAssignableFrom(supportedTypes[j].ContentType)))
                    {
                        var temp = supportedTypes[i];
                        supportedTypes[i] = supportedTypes[j];
                        supportedTypes[j] = temp;
                    }
                }
            }

            return supportedTypes;
        }

        private IEnumerable<FilterContentModelType> FilterContentsWithGenericTypesValueFactory()
        {
            foreach (var filterContent in _filterConfiguration.Filters)
            {
                var contentType = GetContentType(filterContent.Key.GetType());
                yield return new FilterContentModelType { Filter = filterContent.Key, Setting = filterContent.Value, ContentType = contentType ?? typeof(CatalogContentBase), HasGenericArgument = contentType != null };
            }
        }

        private Type GetContentType(Type filterContentType)
        {
            if (filterContentType.Name == typeof (FilterContentBase<,>).Name)
            {
                return filterContentType.GetGenericArguments().First();
            }

            if(filterContentType.GetInterface(typeof (IFilterContent).Name) == null)
            {
                return null;
            }

            return GetContentType(filterContentType.BaseType);
        }
    }
}