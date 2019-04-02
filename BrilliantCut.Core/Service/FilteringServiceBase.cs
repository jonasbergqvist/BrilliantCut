// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilteringServiceBase.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using BrilliantCut.Core.Filters;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer;
    using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Framework.Cache;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Catalog;

    public abstract class FilteringServiceBase<T>
    {
        protected const int MaxItems = 500;

        private const string MasterKey = "BC:FilterContentModelType";

        private const string SearchMethodName = "Search";

        private readonly FilterConfiguration _filterConfiguration;

        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;

        private IEnumerable<FilterContentModelType> _filterContentsWithGenericTypes;

        protected FilteringServiceBase(
            FilterConfiguration filterConfiguration,
            CheckedOptionsService filterModelFactory,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache,
            SearchSortingService searchSorter,
            ReferenceConverter referenceConverter,
            IClient client,
            IContentEvents contentEvents)
        {
            this._filterConfiguration = filterConfiguration;
            this.CheckedOptionsService = filterModelFactory;
            this.ContentRepository = contentRepository;
            this._synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
            this.SearchSortingService = searchSorter;
            this.ReferenceConverter = referenceConverter;
            this.Client = client;

            contentEvents.PublishedContent += (s, e) => CacheManager.Remove(key: MasterKey);
            contentEvents.DeletedContent += (s, e) => CacheManager.Remove(key: MasterKey);
            contentEvents.SavedContent += (s, e) => CacheManager.Remove(key: MasterKey);
        }

        protected CheckedOptionsService CheckedOptionsService { get; private set; }

        protected IClient Client { get; private set; }

        protected IContentRepository ContentRepository { get; private set; }

        protected IEnumerable<FilterContentModelType> FilterContentsWithGenericTypes
        {
            get
            {
                if (this._filterContentsWithGenericTypes == null)
                {
                    List<FilterContentModelType> filterContentModelTypes = new List<FilterContentModelType>();
                    foreach (KeyValuePair<IFilterContent, FacetFilterSetting> filterContent in this._filterConfiguration
                        .Filters)
                    {
                        Type contentType = GetContentType(filterContent.Key.GetType());
                        filterContentModelTypes.Add(
                            new FilterContentModelType
                                {
                                    Filter = filterContent.Key,
                                    Setting = filterContent.Value,
                                    ContentType = contentType ?? typeof(CatalogContentBase),
                                    HasGenericArgument = contentType != null
                                });
                    }

                    this._filterContentsWithGenericTypes = filterContentModelTypes;
                }

                return this._filterContentsWithGenericTypes;
            }
        }

        protected ReferenceConverter ReferenceConverter { get; private set; }

        protected SearchSortingService SearchSortingService { get; private set; }

        protected IServiceLocator ServiceLocator { get; private set; }

        public abstract IEnumerable<T> GetItems(ContentQueryParameters parameters);

        protected virtual void Cache<TCache>(string cacheKey, TCache result, ContentReference contentLink)
            where TCache : class
        {
            this._synchronizedObjectInstanceCache.Insert(
                key: cacheKey,
                value: result,
                evictionPolicy: new CacheEvictionPolicy(
                    null,
                    null,
                    new[]
                        {
                            MasterKey, DataFactoryCache.RootKeyName, "EP:CatalogKeyPricesMasterCacheKey",
                            "Mediachase.Commerce.InventoryService.Storage$MASTER"
                        }));
        }

        protected virtual ISearch CreateSearchQuery(Type contentType)
        {
            // Consider another way of creating an instance of the generic search. Invoke is pretty slow.
            MethodInfo method = typeof(Client).GetMethod(name: SearchMethodName, types: Type.EmptyTypes);
            MethodInfo genericMethod = method.MakeGenericMethod(contentType);
            return genericMethod.Invoke(obj: this.Client, parameters: null) as ISearch;
        }

        protected virtual TCache GetCachedContent<TCache>(string cacheKey)
            where TCache : class
        {
            return this._synchronizedObjectInstanceCache.Get(key: cacheKey) as TCache;
        }

        protected virtual ContentReference GetContentLink(ContentQueryParameters parameters, ListingMode listingMode)
        {
            if (listingMode == ListingMode.WidgetListing)
            {
                return this.ReferenceConverter.GetRootLink();
            }

            return parameters.ReferenceId;
        }

        protected virtual ListingMode GetListingMode(ContentQueryParameters parameters)
        {
            string listingModeString = parameters.AllParameters["listingMode"];
            ListingMode listingMode;
            if (listingModeString != null && Enum.TryParse(value: listingModeString, result: out listingMode))
            {
                return listingMode;
            }

            return ListingMode.NoListing;
        }

        protected virtual Type GetSearchType(FilterModel filterModel, Type restrictedSearchType)
        {
            foreach (KeyValuePair<string, List<object>> filter in filterModel.CheckedItems)
            {
                if (!filter.Value.Any())
                {
                    continue;
                }

                FilterContentModelType filterContentModelType =
                    this.FilterContentsWithGenericTypes.SingleOrDefault(x => x.Filter.Name == filter.Key);

                if (filterContentModelType == null)
                {
                    continue;
                }

                if (restrictedSearchType == null
                    || restrictedSearchType.IsAssignableFrom(c: filterContentModelType.ContentType))
                {
                    restrictedSearchType = filterContentModelType.ContentType;
                }
            }

            return restrictedSearchType;
        }

        protected virtual IEnumerable<FilterContentModelType> GetSupportedFilterContentModelTypes(Type queryType)
        {
            FilterContentModelType[] supportedTypes = this.FilterContentsWithGenericTypes
                .Where(x => x.ContentType.IsAssignableFrom(c: queryType)).ToArray(); // .OrderBy(x => x.Filter.Name)
            for (int i = 0; i < supportedTypes.Length; i++)
            {
                for (int j = i; j < supportedTypes.Length; j++)
                {
                    if (supportedTypes[i].HasGenericArgument
                        && (!supportedTypes[j].HasGenericArgument || supportedTypes[i].ContentType
                                .IsAssignableFrom(c: supportedTypes[j].ContentType)))
                    {
                        FilterContentModelType temp = supportedTypes[i];
                        supportedTypes[i] = supportedTypes[j];
                        supportedTypes[j] = temp;
                    }
                }
            }

            return supportedTypes;
        }

        // protected virtual IFacetContent CreateInstance()
        // {
        // return new FacetContent();
        // }

        // private IEnumerable<FilterContentModelType> FilterContentsWithGenericTypesValueFactory()
        // {
        // var filterContentModelTypes = new List<FilterContentModelType>();
        // foreach (var filterContent in _filterConfiguration.Filters)
        // {
        // var contentType = GetContentType(filterContent.Key.GetType());
        // filterContentModelTypes.Add(new FilterContentModelType { Filter = filterContent.Key, Setting = filterContent.Value, ContentType = contentType ?? typeof(CatalogContentBase), HasGenericArgument = contentType != null });
        // }

        // return filterContentModelTypes;
        // }
        private static Type GetContentType(Type filterContentType)
        {
            if (filterContentType.Name == typeof(FilterContentBase<,>).Name)
            {
                return filterContentType.GetGenericArguments().First();
            }

            if (filterContentType.GetInterface(name: typeof(IFilterContent).Name) == null)
            {
                return null;
            }

            return GetContentType(filterContentType: filterContentType.BaseType);
        }

        protected class FilterContentModelType
        {
            public Type ContentType { get; set; }

            public IFilterContent Filter { get; set; }

            // public bool FacetAdded { get; set; }
            public bool HasGenericArgument { get; set; }

            public FacetFilterSetting Setting { get; set; }
        }
    }
}