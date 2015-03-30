using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using EPiTube.facetFilter.Core;
using EPiTube.FacetFilter.Core.Filters;
using EPiTube.FacetFilter.Core.FilterSettings;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Service
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
            IClient client)
            : base(filterConfiguration, filterModelFactory, contentRepository, synchronizedObjectInstanceCache, searchSorter, client)
        {
        }

        public override IEnumerable<FilterContentWithOptions> GetItems(ContentQueryParameters parameters)
        {
            var filterModelString = parameters.AllParameters["filterModel"];

            var cacheKey = String.Concat("FacetService#", parameters.ReferenceId, "#", filterModelString);
            var cachedResult = GetCachedContent<IEnumerable<FilterContentWithOptions>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var content = ContentRepository.Get<IContent>(parameters.ReferenceId);

            var filters = new Dictionary<string, IEnumerable<object>>();
            var filter = CheckedOptionsService.CreateFilterModel(filterModelString);
            if (filter != null && filter.CheckedItems != null)
            {
                filters = filter.CheckedItems.Where(x => x.Value != null)
                    .ToDictionary(k => k.Key, v => v.Value.Select(x => x));
            }

            var searchType = GetSearchType(filter) ?? typeof(CatalogContentBase);
            var possiblefacetQueries = FilterContentsWithGenericTypes.Value.Where(x =>
                x.ContentType.IsAssignableFrom(searchType) ||
                searchType.IsAssignableFrom(x.ContentType)).ToList();

            var subQueries = new Dictionary<FilterContentModelType, ISearch>();
            AddSubqueries(possiblefacetQueries, subQueries);

            var filterContentModelTypes = GetSupportedFilterContentModelTypes(searchType).ToList();
            AddFiltersToSubQueries(content, subQueries, filterContentModelTypes, filters);

            if (subQueries.Any())
            {
                var result = GetFilterResult(subQueries).ToList();
                Cache(cacheKey, result);

                return result;
            }

            return Enumerable.Empty<FilterContentWithOptions>();
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
                foreach (var otherFilterContentModelType in possiblefacetQueries.Where(otherFilterContentModelType => filterContentModelType.QueryContentType.IsAssignableFrom(otherFilterContentModelType.ContentType)))
                {
                    filterContentModelType.QueryContentType = otherFilterContentModelType.ContentType;
                }

                var subQuery = CreateSearchQuery(filterContentModelType.QueryContentType);
                subQueries.Add(filterContentModelType, subQuery);
            }
        }

        private static void AddFiltersToSubQueries(IContent content, Dictionary<FilterContentModelType, ISearch> subQueries, List<FilterContentModelType> filterContentModelTypes, Dictionary<string, IEnumerable<object>> filters)
        {
            if (filters == null) throw new ArgumentNullException("filters");
            var subQueryFilterContentModelTypes = subQueries.Keys.ToList();
            foreach (var subQueryKey in subQueryFilterContentModelTypes)
            {
                foreach (var filterContentModelType in GetSupportedFilterModelTypes(filterContentModelTypes, subQueryKey))
                {
                    if (ShouldAddFacetToQuery(subQueryKey, filterContentModelType))
                    {
                        subQueries[subQueryKey] = subQueryKey.Filter.AddfacetToQuery(subQueries[subQueryKey]);
                        subQueryKey.FacetAdded = true;

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

                if (!subQueryKey.FacetAdded)
                {
                    subQueries[subQueryKey] = subQueryKey.Filter.AddfacetToQuery(subQueries[subQueryKey]);
                    subQueryKey.FacetAdded = true;
                }
            }
        }

        private static bool ShouldAddFacetToQuery(FilterContentModelType subQueryKey, FilterContentModelType filterContentModelType)
        {
            return !subQueryKey.FacetAdded && filterContentModelType.HasGenericArgument &&
                   (filterContentModelType.Filter.Name == subQueryKey.Filter.Name ||
                    !subQueryKey.ContentType.IsAssignableFrom(filterContentModelType.ContentType));
        }

        private static IEnumerable<FilterContentModelType> GetSupportedFilterModelTypes(IEnumerable<FilterContentModelType> filterContentModelTypes, FilterContentModelType subQueryKey)
        {
            return filterContentModelTypes.Where(x => x.ContentType.IsAssignableFrom(subQueryKey.QueryContentType));
        }

        private IEnumerable<FilterContentWithOptions> GetFilterResult(Dictionary<FilterContentModelType, ISearch> subQueries)
        {
            var filters = new List<FilterContentWithOptions>();

            var multSearch = SearchClient.Instance.MultiSearch<IFacetContent>();
            var filterListInResultOrder = new Dictionary<IFilterContent, FacetFilterSetting>();
            foreach (var subQuery in subQueries.OrderBy(x => x.Key.Filter.Name))
            {
                var typeSearch = subQuery.Value as ITypeSearch<object>;
                AddToMultiSearch(multSearch, typeSearch);

                foreach (var filterContentModelType in FilterContentsWithGenericTypes.Value.Where(x => x.Filter.Name == subQuery.Key.Filter.Name).Where(filterContentModelType => !filters.Select(x => x.FilterContent.Name).Contains(filterContentModelType.Filter.Name)))
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
                    FilterContent = filterListInResultOrderKeys[i],
                    FilterOptions = filterListInResultOrderKeys[i].GetFilterOptions(multiResult[i]).ToArray(),
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

        protected virtual void AddToMultiSearch(IMultiSearch<IFacetContent> multSearch, ITypeSearch<object> typeSearch)
        {
            multSearch.Searches.Add(typeSearch.Select(x => new FacetContent()).Take(0));
        }
    }
}
