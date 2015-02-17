using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Castle.Core.Internal;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor.TinyMCE.Plugins;
using EPiServer.Find;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Framework.Cache;
using EPiServer.Framework.TypeScanner;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;

namespace EPiTube.FasetFilter.Core
{
    [ServiceConfiguration(typeof(FilterContentFactory))]
    public class FilterContentFactory
    {
        private const int MaxItems = 900;
        private const string SearchMethodName = "Search";

        private IClient _client;
        private readonly Lazy<IEnumerable<FilterContentModelType>> _filterContentsWithGenericTypes;
        private readonly ITypeScannerLookup _typeScannerLookup;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;

        public FilterContentFactory(
            ITypeScannerLookup typeScannerLookup, 
            IContentTypeRepository contentTypeRepository,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache)
        {
            _typeScannerLookup = typeScannerLookup;
            _contentTypeRepository = contentTypeRepository;
            _contentRepository = contentRepository;
            _synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;

            _filterContentsWithGenericTypes = new Lazy<IEnumerable<FilterContentModelType>>(FilterContentsWithGenericTypesValueFactory);
        }

        public IClient Client { get { return _client ?? (_client = SearchClient.Instance); } }

        public IEnumerable<FilterContentWithOptions> GetFilters(ContentQueryParameters parameters)
        {
            var result = GetFilteredChildren(parameters);
            
            //var allSupportedFilters = GetFilters(parameters.ReferenceId, result.Filters);
            //result.AddFilters(allSupportedFilters);

            return result.Filters;
        }

        //public IEnumerable<FilterContentWithOptions> GetFilters(ContentReference contentLink, IEnumerable<FilterContentWithOptions> excludeFilters)
        //{
        //    var cacheKey = "EPiTube:GetFilters" + contentLink;
        //    var filters = _synchronizedObjectInstanceCache.Get(cacheKey) as IEnumerable<FilterContentWithOptions>;
        //    if (filters != null)
        //    {
        //        return filters;
        //    }

        //    var excludeFiltersNamed = excludeFilters.Select(x => x.FilterContent.Name).ToArray();
        //    var contentTypes = GetContentTypes(contentLink);

        //    var supportedFilters = new List<FilterContentModelType>();
        //    foreach (var supportedType in contentTypes)
        //    {
        //        supportedFilters.AddRange(
        //            _filterContentsWithGenericTypes.Value
        //            .Where(x =>
        //                !excludeFiltersNamed.Contains(x.Filter.Name) &&
        //                x.ContentType.IsAssignableFrom(supportedType) &&
        //                !supportedFilters.Select(y => y.Filter.Name).Contains(x.Filter.Name)));
        //    }

        //    filters = supportedFilters
        //        .Select(x => new FilterContentWithOptions()
        //        {
        //            FilterContent = x.Filter,
        //            FilterOptions = x.Filter.GetFilterOptions().ToArray()
        //        }).OrderBy(x => x.FilterContent.Name).ToList();

        //    _synchronizedObjectInstanceCache.Insert(
        //        cacheKey,
        //        filters,
        //        new CacheEvictionPolicy(null, null, new[]
        //        {
        //            DataFactoryCache.RootKeyName
        //        },
        //        new TimeSpan(1, 0, 0),
        //        CacheTimeoutType.Sliding));

        //    return filters;
        //}

        protected virtual IEnumerable<Type> GetContentTypes(ContentReference contentLink)
        {
            var cacheKey = "EPiTube:ChildTypes" + contentLink;
            var childTypes = _synchronizedObjectInstanceCache.Get(cacheKey) as IEnumerable<Type>;

            if (childTypes == null)
            {
                var typeIds = SearchClient.Instance.Search<IContent>()
                            .Filter(x => x.Ancestors().Match(contentLink.ToReferenceWithoutVersion().ToString()))
                            .Select(x => x.ContentTypeID)
                            .GetResult();

                childTypes = _contentTypeRepository.List().Where(x => typeIds.Contains(x.ID)).Select(x => x.ModelType);
                _synchronizedObjectInstanceCache.Insert(cacheKey, childTypes, new CacheEvictionPolicy(null, null, new[] { DataFactoryCache.RootKeyName }));
            }

            return childTypes;
        }

        //private ITypeSearch<TSource> OrderBy<TSource, TProperty>(
        //    ITypeSearch<TSource> search,
        //    Expression<Func<TSource, TProperty>> fieldSelector)
        //{
        //    return search.OrderBy(fieldSelector);
        //}

        public EPiTubeModelCollection GetFilteredChildren(ContentQueryParameters parameters)
        {
            var filterModelString = parameters.AllParameters["filterModel"];
            var productGroupedString = parameters.AllParameters["productGrouped"];

            // TODO: Market and other parameters needs to be considered.

            var content = _contentRepository.Get<CatalogContentBase>(parameters.ReferenceId);
            bool productGrouped;
            Boolean.TryParse(productGroupedString, out productGrouped);

            var filter = GetFilterModel(filterModelString);

            return GetFilteredChildren(
                    content,
                    filter,
                    productGrouped,
                    parameters.SortColumns != null ? parameters.SortColumns.FirstOrDefault() : new SortColumn(),
                    parameters.Range);
        }

        private static FilterModel GetFilterModel(string filterModelString)
        {
            if (filterModelString == null)
            {
                return new FilterModel();
            }

            var filterModel = new FilterModel() { Value = new List<FilterContentModel>() };
            FilterContentModel filterContentModel = null;

            var items = filterModelString.Split(new[] { "==" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < items.Length; i++)
            {
                if (i % 2 == 0)
                {
                    filterContentModel = new FilterContentModel()
                    {
                        Name = items[i].Replace("==", string.Empty),
                        Value = new List<FilterContentOptionModel>()
                    };

                    filterModel.Value.Add(filterContentModel);
                }
                else
                {
                    var options = items[i].Split(new[] { ",," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var option in options)
                    {
                        filterContentModel.Value.Add(new FilterContentOptionModel()
                        {
                            Value = option.Replace(",,", string.Empty)
                        });
                    }
                }
            }

            return filterModel;
        }

        //private static object _lock = new object();
        public EPiTubeModelCollection GetFilteredChildren(
            IContent content,
            FilterModel filter,
            bool productGrouped,
            SortColumn sortColumn,
            ItemRange range)
        {
            var searchType = typeof(CatalogContentBase);
            var filters = new Dictionary<string, IEnumerable<object>>();
            if (filter != null && filter.Value != null)
            {
                filters = filter.Value.Where(x => x.Value != null)
                    .ToDictionary(k => k.Name, v => v.Value.Select(x => x.Value));

                var receivedSearchType = GetSearchType(filter);
                if (receivedSearchType != null)
                {
                    searchType = receivedSearchType;
                }
            }

            var includeProductVariationRelations = productGrouped && !(content is ProductContent);
            var supportedFilters = SupportedFilters(searchType).ToList();
            var query = CreateSearchQuery(searchType);

            var possibleFasetQueries = searchType == typeof (CatalogContentBase)
                ? _filterContentsWithGenericTypes.Value
                : _filterContentsWithGenericTypes.Value.Where(x => x.ContentType.IsAssignableFrom(searchType));

            var subQueries = new Dictionary<FilterContentModelType, ITypeSearch<CatalogContentBase>>();
            foreach (var filterContentModelType in possibleFasetQueries)
            {
                //if (supportedFilters.Select(x => x.Name).Contains(filterContentModelType.Filter.Name))
                //{
                //    continue;
                //}

                if (subQueries.ContainsKey(filterContentModelType))
                {
                    subQueries[filterContentModelType] = filterContentModelType.Filter.AddFasetToQuery(subQueries[filterContentModelType]);
                    continue;
                }

                var subQuery = CreateSearchQuery(filterContentModelType.ContentType);
                subQuery = filterContentModelType.Filter.AddFasetToQuery(subQuery);

                subQueries.Add(filterContentModelType, subQuery);
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

            foreach (var supportedFilter in supportedFilters)
            {
                var filterValues = (filters.ContainsKey(supportedFilter.Filter.Name)
                                       ? filters[supportedFilter.Filter.Name]
                                       : Enumerable.Empty<object>()).ToArray();

                query = supportedFilter.Filter.Filter(content, query, filterValues);
                //query = supportedFilter.AddFasetToQuery(query);

                var subQueryFilterContentModelTypes = subQueries.Keys.ToList();
                foreach (var subQueryKey in subQueryFilterContentModelTypes.Where(x => 
                    supportedFilter.Filter.Name != x.Filter.Name &&
                    supportedFilter.ContentType.IsAssignableFrom(x.ContentType)))
                {
                    // TODO: Problem. We need to exclude the once that is not instancable.

                    subQueries[subQueryKey] = supportedFilter.Filter.Filter(content, subQueries[subQueryKey], filterValues);
                    //subQueries[subQueryKey] = Sort(sortColumn, subQueries[subQueryKey]);
                }

                cacheKey += String.Join(";", filterValues);
            }

            query = Sort(sortColumn, query);

            //lock (_lock)
            //{
                var cachedItems = GetCachedContent(cacheKey);
                if (cachedItems != null)
                {
                    range.Total = cachedItems.Item2;
                    return cachedItems.Item1;
                }

                //foreach (var filterContentModelType in _filterContentsWithGenericTypes.Value)
                //{
                //    filterContentModelType.Filter.AddFasetToQuery(query);
                //}

                var properties = _contentRepository.GetDefault<EPiTubeModel>(content.ContentLink).Property;

                var contentList = new EPiTubeModelCollection();
                var linkedProductLinks = new List<ContentReference>();

                var total = AddFilteredChildren(query, subQueries, searchType, content, contentList, linkedProductLinks,
                    supportedFilters, properties, includeProductVariationRelations, startIndex, endIndex);
                range.Total = includeProductVariationRelations ? contentList.Count : total;

                Cache(cacheKey, new Tuple<EPiTubeModelCollection, int>(contentList, total));
                return contentList;
            //}
        }

        private static ITypeSearch<CatalogContentBase> Sort(SortColumn sortColumn, ITypeSearch<CatalogContentBase> query)
        {
            if (String.IsNullOrEmpty(sortColumn.ColumnName))
            {
                return query;
            }

            switch (sortColumn.ColumnName)
            {
                case "name":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                }
                case "code":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.Code()) : query.OrderBy(x => x.Code());
                }
                case "isPendingPublish":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status);
                }
                case "startPublish":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.StartPublish) : query.OrderBy(x => x.StartPublish);
                }
                case "stopPublish":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.StopPublish) : query.OrderBy(x => x.StopPublish);
                }
                case "metaClassName":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.MetaClassId()) : query.OrderBy(x => x.MetaClassId());
                }
                default:
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.ContentTypeID) : query.OrderBy(x => x.ContentTypeID);
                }
            }
        }

        private Tuple<EPiTubeModelCollection, int> GetCachedContent(string cacheKey)
        {
            return _synchronizedObjectInstanceCache.Get(cacheKey) as Tuple<EPiTubeModelCollection, int>;
        }

        private void Cache(string cacheKey, Tuple<EPiTubeModelCollection, int> result)
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
            ITypeSearch<CatalogContentBase> query,
            Dictionary<FilterContentModelType, ITypeSearch<CatalogContentBase>> subQueries,
            Type searchType,
            IContent content,
            EPiTubeModelCollection contentList,
            List<ContentReference> linkedProductLinks,
            IEnumerable<FilterContentModelType> supportedFilters,
            PropertyDataCollection properties,
            bool includeProductVariationRelations, 
            int startIndex, 
            int take)
        {
            try
            {
                var queryResult = query
                .Select(x => new EPiTubeModel
                {
                    PropertyCollection = properties,
                    Name = x.Name,
                    ContentGuid = x.ContentGuid,
                    ContentLink = x.ContentLink,
                    IsDeleted = x.IsDeleted,
                    //StartPublishedNormalized = x.StartPublishedNormalized(),
                    ////LanguageName = x.LanguageName(),
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
                .Skip(startIndex)
                .Take(take + 2)
                .GetResult();

                //contentList.AddFilters(supportedFilters.Select(supportedFilter => new FilterContentWithOptions()
                //{
                //    FilterContent = supportedFilter,
                //    FilterOptions = supportedFilter.GetFilterOptions(queryResult).ToArray()
                //}));

                // TODO: Make as few searches as possible, and do them in a multisearch-query
                foreach (var subQuery in subQueries)
                {
                    var result = subQuery.Value.Select(x => new EPiTubeModel()).Take(0).GetResult();
                    foreach(var filterContentModelType in _filterContentsWithGenericTypes.Value.Where(x => x.Filter.Name == subQuery.Key.Filter.Name))
                    {
                        if (contentList.Filters.Select(x => x.FilterContent.Name).Contains(filterContentModelType.Filter.Name))
                        {
                            continue;
                        }

                        contentList.AddFilter(new FilterContentWithOptions()
                        {
                            FilterContent = filterContentModelType.Filter,
                            FilterOptions = filterContentModelType.Filter.GetFilterOptions(result).ToArray()
                        });
                    }
                }
                
                var total = queryResult.TotalMatching;
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

                    contentList.Add(resultItem);
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
                        total += AddFilteredChildren(productQuery, subQueries, searchType, content, contentList, linkedProductLinks, Enumerable.Empty<FilterContentModelType>(), properties, false, 0, MaxItems);
                    }
                }

                return total;   
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        public Type GetSearchType(FilterModel filterModel)
        {
            Type selectedType = null;
            foreach (var filter in filterModel.Value)
            {
                if (!filter.Value.Any())
                {
                    continue;
                }

                var filterContentModelType = _filterContentsWithGenericTypes.Value
                    .SingleOrDefault(x => x.Filter.Name == filter.Name);

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

        private ITypeSearch<CatalogContentBase> CreateSearchQuery(Type contentType)
        {
            // Consider another way of creating an instance of the generic search. Invoke is pretty slow.
            var method = typeof(Client).GetMethod(SearchMethodName, Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(contentType);
            return genericMethod.Invoke(Client, null) as ITypeSearch<CatalogContentBase>;
        }

        private IEnumerable<FilterContentModelType> SupportedFilters(Type queryType)
        {
            var supportedTypes = _filterContentsWithGenericTypes.Value.Where(x => x.ContentType.IsAssignableFrom(queryType)).ToArray(); //.OrderBy(x => x.Filter.Name)
            //var supportedTypes = _filterContentsWithGenericTypes.Value.Where(x => queryType.IsAssignableFrom(x.ContentType)).ToArray();
            for (var i = 0; i < supportedTypes.Length; i++)
            {
                for (var j = i; j < supportedTypes.Length; j++)
                {
                    if (supportedTypes[i].ContentType.IsAssignableFrom(supportedTypes[j].ContentType))
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
            foreach (var filterContentType in _typeScannerLookup.AllTypes.Where(x => typeof(IFilterContent).IsAssignableFrom(x)))
            {
                // TODO: Replace activator.CreateInstance with fast lambdaexpression with precompilation and cache
                var filterContent = Activator.CreateInstance(filterContentType) as IFilterContent<CatalogContentBase>;
                yield return new FilterContentModelType { Filter = filterContent, ContentType = filterContentType.GetInterface(typeof(IFilterContent<>).Name).GetGenericArguments()[0] };
            }
        }
    }
}