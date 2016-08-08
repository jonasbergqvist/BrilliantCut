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

namespace BrilliantCut.Core.Service
{
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
            IContentEvents contentEvents)
            : base(filterConfiguration, filterModelFactory, contentRepository, synchronizedObjectInstanceCache, searchSorter, referenceConverter, client, contentEvents)
        {
        }

        public override IEnumerable<FilterContentWithOptions> GetItems(ContentQueryParameters parameters)
        {
            var listingMode = GetListingMode(parameters);
            var contentLink = GetContentLink(parameters, listingMode);

            var filterModelString = parameters.AllParameters["filterModel"];
            var searchTypeString = parameters.AllParameters["searchType"];

            var restrictSearchType = !String.IsNullOrEmpty(searchTypeString) ? Type.GetType(searchTypeString) : null;

            var cacheKey = String.Concat("FacetService#", contentLink, "#", filterModelString);
            var cachedResult = GetCachedContent<IEnumerable<FilterContentWithOptions>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var content = ContentRepository.Get<IContent>(contentLink);

            var filters = new Dictionary<string, IEnumerable<object>>();
            var filter = CheckedOptionsService.CreateFilterModel(filterModelString);
            if (filter != null && filter.CheckedItems != null)
            {
                filters = filter.CheckedItems.Where(x => x.Value != null)
                    .ToDictionary(k => k.Key, v => v.Value.Select(x => x));
            }

            var searchType = GetSearchType(filter, restrictSearchType) ?? typeof(CatalogContentBase);
            var possiblefacetQueries = FilterContentsWithGenericTypes.Where(x =>
                x.ContentType.IsAssignableFrom(searchType) ||
                searchType.IsAssignableFrom(x.ContentType)).ToList();

            var subQueries = new Dictionary<FilterContentModelType, ISearch>();
            AddSubqueries(possiblefacetQueries, subQueries, searchType);

            var filterContentModelTypes = GetSupportedFilterContentModelTypes(searchType).ToList();
            AddFiltersToSubQueries(content, subQueries, filterContentModelTypes, filters, searchType);

            if (subQueries.Any())
            {
                var result = GetFilterResult(subQueries, listingMode, content).ToList();
                Cache(cacheKey, result, contentLink);

                return result;
            }

            return Enumerable.Empty<FilterContentWithOptions>();
        }

        private void AddSubqueries(IEnumerable<FilterContentModelType> possiblefacetQueries, Dictionary<FilterContentModelType, ISearch> subQueries, Type searchType)
        {
            foreach (var filterContentModelType in possiblefacetQueries)
            {
                if (subQueries.ContainsKey(filterContentModelType))
                {
                    subQueries[filterContentModelType] = filterContentModelType.Filter.AddfacetToQuery(subQueries[filterContentModelType], filterContentModelType.Setting);
                    continue;
                }

                var typeForQueryCreation = searchType.IsAssignableFrom(filterContentModelType.ContentType)
                    ? filterContentModelType.ContentType
                    : searchType;

                var subQuery = CreateSearchQuery(typeForQueryCreation);
                subQueries.Add(filterContentModelType, subQuery);
            }
        }

        private static void AddFiltersToSubQueries(IContent content, Dictionary<FilterContentModelType, ISearch> subQueries, List<FilterContentModelType> filterContentModelTypes, Dictionary<string, IEnumerable<object>> filters, Type searchType)
        {
            if (filters == null) throw new ArgumentNullException("filters");
            var subQueryFilterContentModelTypes = subQueries.Keys.ToList();
            foreach (var subQueryKey in subQueryFilterContentModelTypes)
            {
                var facetAdded = false;
                foreach (var filterContentModelType in GetSupportedFilterModelTypes(filterContentModelTypes, searchType))
                {
                    if (!facetAdded && ShouldAddFacetToQuery(subQueryKey, filterContentModelType))
                    {
                        subQueries[subQueryKey] = subQueryKey.Filter.AddfacetToQuery(subQueries[subQueryKey], subQueryKey.Setting);
                        facetAdded = true;

                        if (filterContentModelType.Filter.Name == subQueryKey.Filter.Name)
                        {
                            continue;
                        }
                    }

                    var filterValues = (filters.ContainsKey(filterContentModelType.Filter.Name)
                        ? filters[filterContentModelType.Filter.Name]
                        : Enumerable.Empty<object>()).ToArray();

                    subQueries[subQueryKey] = filterContentModelType.Filter.Filter(content, subQueries[subQueryKey], filterValues);
                }

                if (!facetAdded)
                {
                    subQueries[subQueryKey] = subQueryKey.Filter.AddfacetToQuery(subQueries[subQueryKey], subQueryKey.Setting);
                }
            }
        }

        private static bool ShouldAddFacetToQuery(FilterContentModelType subQueryKey, FilterContentModelType filterContentModelType)
        {
            return filterContentModelType.HasGenericArgument &&
                   (filterContentModelType.Filter.Name == subQueryKey.Filter.Name ||
                    !subQueryKey.ContentType.IsAssignableFrom(filterContentModelType.ContentType));
        }

        private static IEnumerable<FilterContentModelType> GetSupportedFilterModelTypes(IEnumerable<FilterContentModelType> filterContentModelTypes, Type searchType)
        {
            return filterContentModelTypes.Where(x => x.ContentType.IsAssignableFrom(searchType));
        }

        private IEnumerable<FilterContentWithOptions> GetFilterResult(Dictionary<FilterContentModelType, ISearch> subQueries, ListingMode listingMode, IContent currentContent)
        {
            var filters = new List<FilterContentWithOptions>();

            var multSearch = SearchClient.Instance.MultiSearch<object>();
            var filterListInResultOrder = new Dictionary<IFilterContent, FacetFilterSetting>();
            foreach (var subQuery in subQueries.OrderBy(x => x.Key.Filter.Name))
            {
                var typeSearch = subQuery.Value as ITypeSearch<object>;
                AddToMultiSearch(multSearch, typeSearch);

                foreach (var filterContentModelType in FilterContentsWithGenericTypes.Where(x => x.Filter.Name == subQuery.Key.Filter.Name).Where(filterContentModelType => !filters.Select(x => x.Name).Contains(filterContentModelType.Filter.Name)))
                {
                    filterListInResultOrder.Add(filterContentModelType.Filter, filterContentModelType.Setting);
                }
            }

            var filterListInResultOrderKeys = filterListInResultOrder.Keys.ToArray();
            var multiResult = multSearch.GetResult().ToList();
            for (var i = 0; i < multiResult.Count; i++)
            {
                var option = new FilterContentWithOptions()
                {
                    Name = filterListInResultOrderKeys[i].Name,
                    FilterContentType = filterListInResultOrderKeys[i].GetType(),
                    FilterOptions = filterListInResultOrderKeys[i].GetFilterOptions(multiResult[i], listingMode, currentContent).ToArray(),
                };

                var settings = filterListInResultOrder[filterListInResultOrderKeys[i]];
                if (settings != null)
                {
                    option.Settings = settings;
                }

                filters.Add(option);
            }

            return filters.OrderBy(x => x.Settings.SortOrder).ToList();
        }

        protected virtual void AddToMultiSearch(IMultiSearch<object> multSearch, ITypeSearch<object> typeSearch)
        {
            multSearch.Searches.Add(typeSearch.Select(x => new object()).Take(0));
        }
    }
}
