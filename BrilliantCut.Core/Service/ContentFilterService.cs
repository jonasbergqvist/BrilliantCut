// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentFilterService.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.Models;

    using EPiServer;
    using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Cms;
    using EPiServer.Framework.Cache;
    using EPiServer.Logging;
    using EPiServer.ServiceLocation;
    using EPiServer.Shell.Services.Rest;

    using Mediachase.Commerce.Catalog;

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
            IContentEvents contentEvents,
            IContentCacheKeyCreator contentCacheKeyCreator)
            : base(
                filterConfiguration: filterConfiguration,
                filterModelFactory: filterModelFactory,
                contentRepository: contentRepository,
                synchronizedObjectInstanceCache: synchronizedObjectInstanceCache,
                searchSorter: searchSorter,
                referenceConverter: referenceConverter,
                client: client,
                contentEvents: contentEvents,
                contentCacheKeyCreator: contentCacheKeyCreator)
        {
        }

        public override IEnumerable<IContent> GetItems(ContentQueryParameters parameters)
        {
            try
            {
                string filterModelString = parameters.AllParameters["filterModel"];
                string productGroupedString = parameters.AllParameters["productGrouped"];
                string searchTypeString = parameters.AllParameters["searchType"];
                SortColumn sortColumn = parameters.SortColumns != null
                                            ? (parameters.SortColumns.FirstOrDefault() ?? new SortColumn())
                                            : new SortColumn();

                bool productGrouped;
                bool.TryParse(value: productGroupedString, result: out productGrouped);
                Type restrictSearchType = !string.IsNullOrEmpty(value: searchTypeString)
                                              ? Type.GetType(typeName: searchTypeString)
                                              : null;

                ListingMode listingMode = this.GetListingMode(parameters: parameters);
                ContentReference contentLink = this.GetContentLink(parameters: parameters, listingMode: listingMode);
                FilterModel filterModel = this.CheckedOptionsService.CreateFilterModel(checkedOptionsString: filterModelString);

                Type searchType = typeof(CatalogContentBase);
                Dictionary<string, IEnumerable<object>> filters = new Dictionary<string, IEnumerable<object>>();

                if (filterModel?.CheckedItems != null)
                {
                    filters = filterModel.CheckedItems.Where(x => x.Value != null)
                        .ToDictionary(k => k.Key, v => v.Value.Select(x => x));

                    Type receivedSearchType = this.GetSearchType(
                        filterModel: filterModel,
                        restrictedSearchType: restrictSearchType);

                    if (receivedSearchType != null)
                    {
                        searchType = receivedSearchType;
                    }
                }

                IContent content = this.ContentRepository.Get<IContent>(contentLink: contentLink);
                bool includeProductVariationRelations = productGrouped && !(content is ProductContent);
                List<FilterContentModelType> supportedFilters = this.GetSupportedFilterContentModelTypes(queryType: searchType).ToList();
                ISearch query = this.CreateSearchQuery(contentType: searchType);

                int startIndex = parameters.Range.Start ?? 0;
                int endIndex = includeProductVariationRelations ? MaxItems : parameters.Range.End ?? MaxItems;

                string cacheKey = string.Concat("ContentFilterService#", content.ContentLink.ToString(), "#");

                if (!string.IsNullOrEmpty(value: sortColumn.ColumnName))
                {
                    cacheKey += string.Format("{0}{1}", sortColumn.ColumnName, sortColumn.SortDescending);
                }

                if (!includeProductVariationRelations)
                {
                    cacheKey += startIndex + endIndex;
                }

                query = GetFiltersToQuery(
                    content: content,
                    supportedFilters: supportedFilters,
                    filters: filters,
                    query: query,
                    cacheKey: ref cacheKey);

                query = this.SearchSortingService.Sort(sortColumn: sortColumn, query: query);

                Tuple<IEnumerable<IFacetContent>, int> cachedItems =
                    this.GetCachedContent<Tuple<IEnumerable<IFacetContent>, int>>(cacheKey: cacheKey);

                if (cachedItems != null)
                {
                    parameters.Range.Total = cachedItems.Item2;
                    return cachedItems.Item1;
                }

                PropertyDataCollection properties = this.ContentRepository
                    .GetDefault<FacetContent>(parentLink: content.ContentLink).Property;

                List<IFacetContent> contentList = new List<IFacetContent>();
                List<ContentReference> linkedProductLinks = new List<ContentReference>();

                int total = this.AddFilteredChildren(
                    query: query,
                    searchType: restrictSearchType,
                    contentList: contentList,
                    linkedProductLinks: linkedProductLinks,
                    properties: properties,
                    includeProductVariationRelations: includeProductVariationRelations,
                    startIndex: startIndex,
                    take: endIndex);

                parameters.Range.Total = includeProductVariationRelations ? contentList.Count : total;

                this.Cache(
                    cacheKey: cacheKey,
                    result: new Tuple<IEnumerable<IFacetContent>, int>(
                        item1: contentList,
                        item2: parameters.Range.Total.Value),
                    contentLink: contentLink);
                return contentList;
            }
            catch (Exception exception)
            {
                this.Logger.Log(level: Level.Error, message: exception.Message, exception: exception);
            }
            
            return new List<IContent>();
        }

        protected virtual IEnumerable<IFacetContent> GetSearchResults(
            ITypeSearch<CatalogContentBase> query,
            Type searchType,
            PropertyDataCollection properties,
            int skip,
            int take,
            out int total)
        {
            if (searchType != null)
            {
                query = query.Filter(x => x.MatchTypeHierarchy(searchType));
            }

            SearchResults<SearchFacetContent> result = query

                // .Filter(x => x.MatchTypeHierarchy())
                .Skip(skip: skip).Take(take: take).Select(
                    x => new SearchFacetContent
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

                // .IncludeType<FacetContent, IFacetContent>(x =>
                // new FacetContent
                // {
                // PropertyCollection = properties,
                // Name = x.Name,
                // ContentGuid = x.ContentGuid,
                // ContentLink = x.ContentLink,
                // IsDeleted = x.IsDeleted,
                // VariationLinks = x.VariationLinks(),
                // ParentLink = x.ParentLink,
                // StartPublish = x.StartPublish,
                // StopPublish = x.StopPublish,
                // Code = x.Code,
                // DefaultPriceValue = x.DefaultPriceValue,
                // ContentTypeID = x.ContentTypeID,
                // ApplicationId = x.ApplicationId,
                // MetaClassId = x.MetaClassId,
                // ProductLinks = x.ProductLinks(),
                // NodeLinks = x.NodeLinks(),
                // LinkUrl = x.LinkUrl,
                // ThumbnailPath = x.ThumbnailPath,
                // DefaultImageUrl = x.DefaultImageUrl,
                // DefaultCurrency = x.DefaultCurrency,
                // WeightBase = x.WeightBase,
                // LengthBase = x.LengthBase,
                // Prices = x.Prices(),
                // Inventories = x.Inventories(),
                // CategoryNames = x.CategoryNames
                // })
                .GetResult();

            total = result.TotalMatching;
            return result.Select(
                x => new FacetContent
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
                             DefaultPriceValue = x.DefaultPriceValue,
                             ContentTypeID = x.ContentTypeID,
                             MetaClassId = x.MetaClassId,
                             ProductLinks = x.ProductLinks,
                             NodeLinks = x.NodeLinks,
                             ThumbnailPath = x.ThumbnailPath,
                             LinkUrl = x.LinkUrl,
                             DefaultImageUrl = x.DefaultImageUrl,
                             DefaultCurrency = x.DefaultCurrency,
                             WeightBase = x.WeightBase,
                             LengthBase = x.LengthBase,
                             Prices = x.Prices,
                             Inventories = x.Inventories
                         });
        }

        private static ISearch GetFiltersToQuery(
            IContent content,
            IEnumerable<FilterContentModelType> supportedFilters,
            Dictionary<string, IEnumerable<object>> filters,
            ISearch query,
            ref string cacheKey)
        {
            foreach (FilterContentModelType supportedFilter in supportedFilters)
            {
                object[] filterValues = (filters.ContainsKey(key: supportedFilter.Filter.Name)
                                             ? filters[key: supportedFilter.Filter.Name]
                                             : Enumerable.Empty<object>()).ToArray();

                query = supportedFilter.Filter.Filter(content: content, query: query, values: filterValues);

                cacheKey += string.Join(";", values: filterValues);
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
                List<IFacetContent> queryResult = this.GetSearchResult(
                    query: query,
                    searchType: searchType,
                    properties: properties,
                    startIndex: startIndex,
                    take: take,
                    total: out total).ToList();
                foreach (IFacetContent resultItem in queryResult)
                {
                    // When we ask for relations, add product links to linkedProductList if any exists, and do not add a model for the content if it has product links.
                    if (includeProductVariationRelations)
                    {
                        if (resultItem.ProductLinks != null && resultItem.ProductLinks.Any())
                        {
                            foreach (ContentReference productLink in resultItem.ProductLinks.Where(
                                productLink => !linkedProductLinks.Contains(item: productLink)))
                            {
                                linkedProductLinks.Add(item: productLink);
                            }

                            continue;
                        }
                    }

                    if (!contentList.Any(
                            x => x.ContentLink.CompareToIgnoreWorkID(contentReference: resultItem.ContentLink)))
                    {
                        contentList.Add(item: resultItem);
                    }
                }

                if (includeProductVariationRelations)
                {
                    // get parent products in the query result
                    IEnumerable<ContentReference> addedContentLinks = contentList.Select(x => x.ContentLink);
                    IEnumerable<ContentReference> links = addedContentLinks;
                    ContentReference[] notAddedLinkedProducts =
                        linkedProductLinks.Where(x => !links.Contains(value: x)).ToArray();
                    foreach (IFacetContent resultItem in notAddedLinkedProducts
                        .Select(
                            linkedProductLink => queryResult.FirstOrDefault(
                                x => x.ContentLink.CompareToIgnoreWorkID(contentReference: linkedProductLink)))
                        .Where(resultItem => resultItem != null))
                    {
                        contentList.Add(item: resultItem);
                    }

                    addedContentLinks = contentList.Select(x => x.ContentLink);
                    notAddedLinkedProducts =
                        linkedProductLinks.Where(x => !addedContentLinks.Contains(value: x)).ToArray();
                    if (notAddedLinkedProducts.Any())
                    {
                        FilterBuilder<ProductContent> filterBuilder =
                            new FilterBuilder<ProductContent>(client: this.Client);
                        filterBuilder = notAddedLinkedProducts.Aggregate(
                            seed: filterBuilder,
                            func: (current, reference) =>
                                current.Or(x => x.ContentLink.Match(reference)));

                        ITypeSearch<ProductContent> productQuery =
                            this.Client.Search<ProductContent>().Filter(filter: filterBuilder);
                        total += this.AddFilteredChildren(
                            query: productQuery,
                            searchType: searchType,
                            contentList: contentList,
                            linkedProductLinks: linkedProductLinks,
                            properties: properties,
                            includeProductVariationRelations: false,
                            startIndex: 0,
                            take: MaxItems);
                    }
                }

                return total;
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        private IEnumerable<IFacetContent> GetSearchResult(
            ISearch query,
            Type searchType,
            PropertyDataCollection properties,
            int startIndex,
            int take,
            out int total)
        {
            ITypeSearch<CatalogContentBase> catalogContentSearch = query as ITypeSearch<CatalogContentBase>;

            if (catalogContentSearch != null)
            {
                return this.GetSearchResults(
                    query: catalogContentSearch,
                    searchType: searchType,
                    properties: properties,
                    skip: startIndex,
                    take: take + 2,
                    total: out total);
            }

            throw new NotSupportedException(
                "The type needs to inherit from CatalogContentBase, or implement IFacetContent");
        }
    }
}