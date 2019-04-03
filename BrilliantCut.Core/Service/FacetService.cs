// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetService.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.Filters;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer;
    using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Framework;
    using EPiServer.Framework.Cache;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Catalog;

    [ServiceConfiguration(typeof(FacetService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class FacetService : FilteringServiceBase<FilterContentWithOptions>
    {
        public FacetService(
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
                contentCacheKeyCreator:contentCacheKeyCreator)
        {
        }

        public override IEnumerable<FilterContentWithOptions> GetItems(ContentQueryParameters parameters)
        {
            ListingMode listingMode = this.GetListingMode(parameters: parameters);
            ContentReference contentLink = this.GetContentLink(parameters: parameters, listingMode: listingMode);

            string filterModelString = parameters.AllParameters["filterModel"];
            string searchTypeString = parameters.AllParameters["searchType"];

            Type restrictSearchType = !string.IsNullOrEmpty(value: searchTypeString)
                                          ? Type.GetType(typeName: searchTypeString)
                                          : null;

            string cacheKey = string.Concat("FacetService#", contentLink, "#", filterModelString);
            IEnumerable<FilterContentWithOptions> cachedResult =
                this.GetCachedContent<IEnumerable<FilterContentWithOptions>>(cacheKey: cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            IContent content = this.ContentRepository.Get<IContent>(contentLink: contentLink);

            Dictionary<string, IEnumerable<object>> filters = new Dictionary<string, IEnumerable<object>>();
            FilterModel filter = this.CheckedOptionsService.CreateFilterModel(checkedOptionsString: filterModelString);
            if (filter != null && filter.CheckedItems != null)
            {
                filters = filter.CheckedItems.Where(x => x.Value != null)
                    .ToDictionary(k => k.Key, v => v.Value.Select(x => x));
            }

            Type searchType = this.GetSearchType(filterModel: filter, restrictedSearchType: restrictSearchType)
                              ?? typeof(CatalogContentBase);
            List<FilterContentModelType> possiblefacetQueries = this.FilterContentsWithGenericTypes.Where(
                    x => x.ContentType.IsAssignableFrom(c: searchType) || searchType.IsAssignableFrom(c: x.ContentType))
                .ToList();

            Dictionary<FilterContentModelType, ISearch> subQueries = new Dictionary<FilterContentModelType, ISearch>();
            this.AddSubqueries(
                possiblefacetQueries: possiblefacetQueries,
                subQueries: subQueries,
                searchType: searchType);

            List<FilterContentModelType> filterContentModelTypes =
                this.GetSupportedFilterContentModelTypes(queryType: searchType).ToList();
            AddFiltersToSubQueries(
                content: content,
                subQueries: subQueries,
                filterContentModelTypes: filterContentModelTypes,
                filters: filters,
                searchType: searchType);

            if (subQueries.Any())
            {
                List<FilterContentWithOptions> result = this.GetFilterResult(
                    subQueries: subQueries,
                    listingMode: listingMode,
                    currentContent: content).ToList();
                this.Cache(cacheKey: cacheKey, result: result, contentLink: contentLink);

                return result;
            }

            return Enumerable.Empty<FilterContentWithOptions>();
        }

        protected virtual void AddToMultiSearch(IMultiSearch<object> multSearch, ITypeSearch<object> typeSearch)
        {
            multSearch.Searches.Add(typeSearch.Select(x => new object()).Take(0));
        }

        private static void AddFiltersToSubQueries(
            IContent content,
            Dictionary<FilterContentModelType, ISearch> subQueries,
            List<FilterContentModelType> filterContentModelTypes,
            Dictionary<string, IEnumerable<object>> filters,
            Type searchType)
        {
            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }

            List<FilterContentModelType> subQueryFilterContentModelTypes = subQueries.Keys.ToList();
            foreach (FilterContentModelType subQueryKey in subQueryFilterContentModelTypes)
            {
                bool facetAdded = false;
                foreach (FilterContentModelType filterContentModelType in GetSupportedFilterModelTypes(
                    filterContentModelTypes: filterContentModelTypes,
                    searchType: searchType))
                {
                    if (!facetAdded && ShouldAddFacetToQuery(
                            subQueryKey: subQueryKey,
                            filterContentModelType: filterContentModelType))
                    {
                        subQueries[key: subQueryKey] = subQueryKey.Filter.AddFacetToQuery(
                            subQueries[key: subQueryKey],
                            setting: subQueryKey.Setting);
                        facetAdded = true;

                        if (filterContentModelType.Filter.Name == subQueryKey.Filter.Name)
                        {
                            continue;
                        }
                    }

                    object[] filterValues = (filters.ContainsKey(key: filterContentModelType.Filter.Name)
                                                 ? filters[key: filterContentModelType.Filter.Name]
                                                 : Enumerable.Empty<object>()).ToArray();

                    subQueries[key: subQueryKey] = filterContentModelType.Filter.Filter(
                        content: content,
                        query: subQueries[key: subQueryKey],
                        values: filterValues);
                }

                if (!facetAdded)
                {
                    subQueries[key: subQueryKey] = subQueryKey.Filter.AddFacetToQuery(
                        subQueries[key: subQueryKey],
                        setting: subQueryKey.Setting);
                }
            }
        }

        private static IEnumerable<FilterContentModelType> GetSupportedFilterModelTypes(
            IEnumerable<FilterContentModelType> filterContentModelTypes,
            Type searchType)
        {
            return filterContentModelTypes.Where(x => x.ContentType.IsAssignableFrom(c: searchType));
        }

        private static bool ShouldAddFacetToQuery(
            FilterContentModelType subQueryKey,
            FilterContentModelType filterContentModelType)
        {
            return filterContentModelType.HasGenericArgument
                   && (filterContentModelType.Filter.Name == subQueryKey.Filter.Name
                       || !subQueryKey.ContentType.IsAssignableFrom(c: filterContentModelType.ContentType));
        }

        private void AddSubqueries(
            IEnumerable<FilterContentModelType> possiblefacetQueries,
            Dictionary<FilterContentModelType, ISearch> subQueries,
            Type searchType)
        {
            foreach (FilterContentModelType filterContentModelType in possiblefacetQueries)
            {
                if (subQueries.ContainsKey(key: filterContentModelType))
                {
                    subQueries[key: filterContentModelType] = filterContentModelType.Filter.AddFacetToQuery(
                        subQueries[key: filterContentModelType],
                        setting: filterContentModelType.Setting);
                    continue;
                }

                Type typeForQueryCreation = searchType.IsAssignableFrom(c: filterContentModelType.ContentType)
                                                ? filterContentModelType.ContentType
                                                : searchType;

                ISearch subQuery = this.CreateSearchQuery(contentType: typeForQueryCreation);

                if (subQuery != null)
                {
                    subQueries.Add(key: filterContentModelType, value: subQuery);
                }
            }
        }

        private IEnumerable<FilterContentWithOptions> GetFilterResult(
            Dictionary<FilterContentModelType, ISearch> subQueries,
            ListingMode listingMode,
            IContent currentContent)
        {
            List<FilterContentWithOptions> filters = new List<FilterContentWithOptions>();

            IMultiSearch<object> multSearch = SearchClient.Instance.MultiSearch<object>();
            Dictionary<IFilterContent, FacetFilterSetting> filterListInResultOrder =
                new Dictionary<IFilterContent, FacetFilterSetting>();
            foreach (KeyValuePair<FilterContentModelType, ISearch> subQuery in subQueries.OrderBy(
                x => x.Key.Filter.Name))
            {
                ITypeSearch<object> typeSearch = subQuery.Value as ITypeSearch<object>;
                this.AddToMultiSearch(multSearch: multSearch, typeSearch: typeSearch);

                foreach (FilterContentModelType filterContentModelType in this.FilterContentsWithGenericTypes
                    .Where(x => x.Filter.Name == subQuery.Key.Filter.Name).Where(
                        filterContentModelType =>
                            !filters.Select(x => x.Name).Contains(value: filterContentModelType.Filter.Name)))
                {
                    filterListInResultOrder.Add(
                        key: filterContentModelType.Filter,
                        value: filterContentModelType.Setting);
                }
            }

            IFilterContent[] filterListInResultOrderKeys = filterListInResultOrder.Keys.ToArray();
            List<SearchResults<object>> multiResult = multSearch.GetResult().ToList();
            for (int i = 0; i < multiResult.Count; i++)
            {
                FilterContentWithOptions option = new FilterContentWithOptions
                                                      {
                                                          Name = filterListInResultOrderKeys[i].Name,
                                                          FilterContentType = filterListInResultOrderKeys[i].GetType(),
                                                          FilterOptions = filterListInResultOrderKeys[i]
                                                              .GetFilterOptions(
                                                                  multiResult[index: i],
                                                                  mode: listingMode,
                                                                  currentContent: currentContent).ToArray()
                                                      };

                FacetFilterSetting settings = filterListInResultOrder[filterListInResultOrderKeys[i]];
                if (settings != null)
                {
                    option.Settings = settings;
                }

                filters.Add(item: option);
            }

            return filters.OrderBy(x => x.Settings.SortOrder).ToList();
        }
    }
}