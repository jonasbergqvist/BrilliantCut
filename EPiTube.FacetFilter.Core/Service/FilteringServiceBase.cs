using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using EPiTube.facetFilter.Core;
using EPiTube.FacetFilter.Core.Filters;
using EPiTube.FacetFilter.Core.FilterSettings;
using EPiTube.FacetFilter.Core.Models;
using Mediachase.Commerce.Catalog;

namespace EPiTube.FacetFilter.Core.Service
{
    public abstract class FilteringServiceBase<T>
    {
        protected class FilterContentModelType
        {
            public IFilterContent Filter { get; set; }
            public Type ContentType { get; set; }
            public bool FacetAdded { get; set; }
            public bool HasGenericArgument { get; set; }
            public FacetFilterSetting Setting { get; set; }
        }

        protected const int MaxItems = 500;
        private const string SearchMethodName = "Search";

        private readonly FilterConfiguration _filterConfiguration;
        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;

        protected FilteringServiceBase(
            FilterConfiguration filterConfiguration,
            CheckedOptionsService filterModelFactory,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache,
            SearchSortingService searchSorter,
            ReferenceConverter referenceConverter,
            IClient client)
        {
            _filterConfiguration = filterConfiguration;
            CheckedOptionsService = filterModelFactory;
            ContentRepository = contentRepository;
            _synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
            SearchSortingService = searchSorter;
            ReferenceConverter = referenceConverter;
            Client = client;

            FilterContentsWithGenericTypes = new Lazy<IEnumerable<FilterContentModelType>>(FilterContentsWithGenericTypesValueFactory, false);
        }

        protected ReferenceConverter ReferenceConverter { get; private set; }
        protected IClient Client { get; private set; }
        protected IContentRepository ContentRepository { get; private set; }
        protected CheckedOptionsService CheckedOptionsService { get; private set; }
        protected SearchSortingService SearchSortingService { get; private set; }
        protected Lazy<IEnumerable<FilterContentModelType>> FilterContentsWithGenericTypes { get; private set; }
        protected IServiceLocator ServiceLocator { get; private set; }

        public abstract IEnumerable<T> GetItems(ContentQueryParameters parameters);

        protected virtual TCache GetCachedContent<TCache>(string cacheKey)
            where TCache : class
        {
            return _synchronizedObjectInstanceCache.Get(cacheKey) as TCache;
        }

        protected virtual void Cache<TCache>(string cacheKey, TCache result)
            where TCache : class
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

        protected virtual ListingMode GetListingMode(ContentQueryParameters parameters)
        {
            var listingModeString = parameters.AllParameters["listingMode"];
            ListingMode listingMode;
            if (listingModeString != null && Enum.TryParse(listingModeString, out listingMode))
            {
                return listingMode;
            }

            return ListingMode.NoListing;
        }

        protected virtual ContentReference GetContentLink(ContentQueryParameters parameters, ListingMode listingMode)
        {
            if (listingMode == ListingMode.WidgetListing)
            {
                return ReferenceConverter.GetRootLink();
            }

            return parameters.ReferenceId;
        }

        protected virtual Type GetSearchType(FilterModel filterModel)
        {
            Type selectedType = null;
            foreach (var filter in filterModel.CheckedItems)
            {
                if (!filter.Value.Any())
                {
                    continue;
                }

                var filterContentModelType = FilterContentsWithGenericTypes.Value
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

        protected virtual ISearch CreateSearchQuery(Type contentType)
        {
            // Consider another way of creating an instance of the generic search. Invoke is pretty slow.
            var method = typeof(Client).GetMethod(SearchMethodName, Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(contentType);
            return genericMethod.Invoke(Client, null) as ISearch;
        }

        protected virtual IEnumerable<FilterContentModelType> GetSupportedFilterContentModelTypes(Type queryType)
        {
            var supportedTypes = FilterContentsWithGenericTypes.Value.Where(x => x.ContentType.IsAssignableFrom(queryType)).ToArray(); //.OrderBy(x => x.Filter.Name)
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

        protected virtual IFacetContent CreateInstance()
        {
            return new FacetContent();
        }

        private IEnumerable<FilterContentModelType> FilterContentsWithGenericTypesValueFactory()
        {
            foreach (var filterContent in _filterConfiguration.Filters)
            {
                var contentType = GetContentType(filterContent.Key.GetType());
                yield return new FilterContentModelType { Filter = filterContent.Key, Setting = filterContent.Value, ContentType = contentType ?? typeof(CatalogContentBase), HasGenericArgument = contentType != null };
            }
        }

        private static Type GetContentType(Type filterContentType)
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