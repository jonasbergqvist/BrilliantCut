using System;
using System.Collections.Generic;
using System.Linq;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.Models;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;
using Mediachase.Commerce.Catalog;

namespace BrilliantCut.Core.Service
{
    [ServiceConfiguration(typeof(ContentFilterService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class ContentFilterService : FilteringServiceBase<IContent>
    {
        public ContentFilterService(
            FilterConfiguration filterConfiguration,
            CheckedOptionsService filterModelFactory,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache,
            SearchSortingService searchSorter,
            ReferenceConverter referenceConverter,
            IClient client,
            IContentEvents contentEvents)
            : base(filterConfiguration, filterModelFactory, contentRepository, synchronizedObjectInstanceCache, searchSorter, referenceConverter, client, contentEvents)
        {
        }

        public override IEnumerable<IContent> GetItems(EPiServer.Cms.Shell.UI.Rest.ContentQuery.ContentQueryParameters parameters)
        {
            var filterModelString = parameters.AllParameters["filterModel"];
            var productGroupedString = parameters.AllParameters["productGrouped"];
            var searchTypeString = parameters.AllParameters["searchType"];
            var sortColumn = parameters.SortColumns != null ? (parameters.SortColumns.FirstOrDefault() ?? new SortColumn()) : new SortColumn();

            bool productGrouped;
            Boolean.TryParse(productGroupedString, out productGrouped);
            var restrictSearchType = !String.IsNullOrEmpty(searchTypeString) ? Type.GetType(searchTypeString) : null;

            var listingMode = GetListingMode(parameters);
            var contentLink = GetContentLink(parameters, listingMode);
            var filterModel = CheckedOptionsService.CreateFilterModel(filterModelString);

            var searchType = typeof(CatalogContentBase); 
            var filters = new Dictionary<string, IEnumerable<object>>();
            if (filterModel != null && filterModel.CheckedItems != null)
            {
                filters = filterModel.CheckedItems.Where(x => x.Value != null)
                    .ToDictionary(k => k.Key, v => v.Value.Select(x => x));

                var receivedSearchType = GetSearchType(filterModel, restrictSearchType);
                if (receivedSearchType != null)
                {
                    searchType = receivedSearchType;
                }
            }

            var content = ContentRepository.Get<IContent>(contentLink);
            var includeProductVariationRelations = productGrouped && !(content is ProductContent);
            var supportedFilters = GetSupportedFilterContentModelTypes(searchType).ToList();
            var query = CreateSearchQuery(searchType);

            var startIndex = parameters.Range.Start ?? 0;
            var endIndex = includeProductVariationRelations ? MaxItems : parameters.Range.End ?? MaxItems;

            var cacheKey = String.Concat("ContentFilterService#", content.ContentLink.ToString(), "#");
            if (!String.IsNullOrEmpty(sortColumn.ColumnName))
            {
                cacheKey += sortColumn.ColumnName + sortColumn.SortDescending;
            }

            if (!includeProductVariationRelations)
            {
                cacheKey += startIndex + endIndex;
            }

            query = GetFiltersToQuery(content, supportedFilters, filters, query, ref cacheKey);
            query = SearchSortingService.Sort(sortColumn, query);

            var cachedItems = GetCachedContent<Tuple<IEnumerable<IFacetContent>, int>>(cacheKey);
            if (cachedItems != null)
            {
                parameters.Range.Total = cachedItems.Item2;
                return cachedItems.Item1;
            }

            var properties = ContentRepository.GetDefault<FacetContent>(content.ContentLink).Property;

            var contentList = new List<IFacetContent>();
            var linkedProductLinks = new List<ContentReference>();

            var total = AddFilteredChildren(query, restrictSearchType, contentList, linkedProductLinks,
                properties, includeProductVariationRelations, startIndex, endIndex);

            parameters.Range.Total = includeProductVariationRelations ? contentList.Count : total;

            Cache(cacheKey, new Tuple<IEnumerable<IFacetContent>, int>(contentList, parameters.Range.Total.Value), contentLink);
            return contentList;
        }

        private static ISearch GetFiltersToQuery(IContent content, IEnumerable<FilterContentModelType> supportedFilters, Dictionary<string, IEnumerable<object>> filters, ISearch query, ref string cacheKey)
        {
            foreach (var supportedFilter in supportedFilters)
            {
                var filterValues = (filters.ContainsKey(supportedFilter.Filter.Name)
                    ? filters[supportedFilter.Filter.Name]
                    : Enumerable.Empty<object>()).ToArray();

                query = supportedFilter.Filter.Filter(content, query, filterValues);

                cacheKey += String.Join(";", filterValues);
            }

            return query;
        }

        private int AddFilteredChildren(
            ISearch query, 
            Type searchType,
            ICollection<IFacetContent> contentList,
            ICollection<ContentReference> linkedProductLinks,
            PropertyDataCollection properties,
            bool includeProductVariationRelations,
            int startIndex,
            int take)
        {
            try
            {
                int total;
                var queryResult = GetSearchResult(query, searchType, properties, startIndex, take, out total).ToList();
                foreach (var resultItem in queryResult)
                {
                    // When we ask for relations, add product links to linkedProductList if any exists, and do not add a model for the content if it has product links.
                    if (includeProductVariationRelations)
                    {
                        if (resultItem.ProductLinks != null && resultItem.ProductLinks.Any())
                        {
                            foreach (var productLink in resultItem.ProductLinks.Where(productLink => !linkedProductLinks.Contains(productLink)))
                            {
                                linkedProductLinks.Add(productLink);
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
                    var notAddedLinkedProducts = linkedProductLinks.Where(x => !links.Contains(x)).ToArray();
                    foreach (var resultItem in notAddedLinkedProducts.Select(linkedProductLink => queryResult.FirstOrDefault(x => x.ContentLink.CompareToIgnoreWorkID(linkedProductLink))).Where(resultItem => resultItem != null))
                    {
                        contentList.Add(resultItem);
                    }

                    addedContentLinks = contentList.Select(x => x.ContentLink);
                    notAddedLinkedProducts = linkedProductLinks.Where(x => !addedContentLinks.Contains(x)).ToArray();
                    if (notAddedLinkedProducts.Any())
                    {
                        var filterBuilder = new FilterBuilder<ProductContent>(Client);
                        filterBuilder = notAddedLinkedProducts.Aggregate(filterBuilder,
                            (current, reference) => current.Or(x => x.ContentLink.Match(reference)));

                        var productQuery = Client.Search<ProductContent>().Filter(filterBuilder);
                        total += AddFilteredChildren(productQuery, searchType, contentList,
                            linkedProductLinks, properties, false, 0,
                            MaxItems);
                    }
                }

                return total;
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        private IEnumerable<IFacetContent> GetSearchResult(ISearch query, Type searchType, PropertyDataCollection properties, int startIndex, int take, out int total)
        {
            var catalogContentSearch = query as ITypeSearch<CatalogContentBase>;
            if (catalogContentSearch != null)
            {
                return GetSearchResults(catalogContentSearch, searchType, properties, startIndex, take + 2, out total);
            }

            throw new NotSupportedException(
                "The type needs to inherit from CatalogContentBase, or implement IFacetContent");
        }

        protected virtual IEnumerable<IFacetContent> GetSearchResults(ITypeSearch<CatalogContentBase> query, Type searchType, PropertyDataCollection properties, int skip, int take, out int total)
        {
            if (searchType != null)
            {
                query = query.Filter(x => x.MatchTypeHierarchy(searchType));
            }

            var result = query
                //.Filter(x => x.MatchTypeHierarchy())
                .Skip(skip)
                .Take(take)
                .Select(x => new FacetContent
                {
                    PropertyCollection = properties,
                    Name = x.Name,
                    ContentGuid = x.ContentGuid,
                    ContentLink = x.ContentLink,
                    IsDeleted = x.IsDeleted,
                    VariationLinks = x.Variations(),
                    ParentLink = x.ParentLink,
                    StartPublish = x.StartPublish,
                    StopPublish = x.StopPublish,
                    Code = x.Code(),
                    DefaultPriceValue = x.DefaultPriceValue(),
                    ContentTypeID = x.ContentTypeID,
                    ApplicationId = x.ApplicationId,
                    MetaClassId = x.MetaClassId(),
                    ProductLinks = x.ParentProducts(),
                    NodeLinks = x.NodeLinks(),
                    ThumbnailPath = x.ThumbnailUrl(),
                    LinkUrl = x.LinkUrl(),
                    DefaultImageUrl = x.DefaultImageUrl(),
                    DefaultCurrency = x.DefaultCurrency(),
                    WeightBase = x.WeightBase(),
                    LengthBase = x.LengthBase(),
                    Prices = x.Prices(),
                    Inventories = x.Inventories()
                })
                .IncludeType<FacetContent, IFacetContent>(x =>
                    new FacetContent
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
                        Code = x.Code,
                        DefaultPriceValue = x.DefaultPriceValue,
                        ContentTypeID = x.ContentTypeID,
                        ApplicationId = x.ApplicationId,
                        MetaClassId = x.MetaClassId,
                        ProductLinks = x.ProductLinks(),
                        NodeLinks = x.NodeLinks(),
                        LinkUrl = x.LinkUrl,
                        ThumbnailPath = x.ThumbnailPath,
                        DefaultImageUrl = x.DefaultImageUrl,
                        DefaultCurrency = x.DefaultCurrency,
                        WeightBase = x.WeightBase,
                        LengthBase = x.LengthBase,
                        Prices = x.Prices(),
                        Inventories = x.Inventories(),
                        CategoryNames = x.CategoryNames
                    })
                .GetResult();

            total = result.TotalMatching;
            return result;
        }
    }
}
