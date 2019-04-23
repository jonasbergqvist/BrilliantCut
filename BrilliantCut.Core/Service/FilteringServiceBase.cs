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
    using EPiServer.Logging;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Catalog;

    /// <summary>
    /// Class FilteringServiceBase.
    /// </summary>
    /// <typeparam name="T">The type of filter</typeparam>
    public abstract class FilteringServiceBase<T>
    {
        /// <summary>
        /// The maximum items
        /// </summary>
        protected const int MaxItems = 500;

        /// <summary>
        /// The master key
        /// </summary>
        private const string MasterKey = "BC:FilterContentModelType";

        /// <summary>
        /// The search method name
        /// </summary>
        private const string SearchMethodName = "Search";

        /// <summary>
        /// The filter configuration
        /// </summary>
        private readonly FilterConfiguration filterConfiguration;

        /// <summary>
        /// The synchronized object instance cache
        /// </summary>
        private readonly ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache;

        /// <summary>
        /// The content cache key creator
        /// </summary>
        private readonly IContentCacheKeyCreator contentCacheKeyCreator;

        /// <summary>
        /// The filter contents with generic types
        /// </summary>
        private IEnumerable<FilterContentModelType> filterContentsWithGenericTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteringServiceBase{T}"/> class.
        /// </summary>
        /// <param name="filterConfiguration">The filter configuration.</param>
        /// <param name="filterModelFactory">The filter model factory.</param>
        /// <param name="contentRepository">The content repository.</param>
        /// <param name="synchronizedObjectInstanceCache">The synchronized object instance cache.</param>
        /// <param name="searchSorter">The search sorter.</param>
        /// <param name="referenceConverter">The reference converter.</param>
        /// <param name="client">The client.</param>
        /// <param name="contentEvents">The content events.</param>
        /// <param name="contentCacheKeyCreator">The content cache key creator.</param>
        protected FilteringServiceBase(
            FilterConfiguration filterConfiguration,
            CheckedOptionsService filterModelFactory,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache,
            SearchSortingService searchSorter,
            ReferenceConverter referenceConverter,
            IClient client,
            IContentEvents contentEvents,
            IContentCacheKeyCreator contentCacheKeyCreator)
        {
            this.filterConfiguration = filterConfiguration;
            this.CheckedOptionsService = filterModelFactory;
            this.ContentRepository = contentRepository;
            this.synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;
            this.SearchSortingService = searchSorter;
            this.ReferenceConverter = referenceConverter;
            this.Client = client;
            this.Logger = LogManager.GetLogger();
            this.contentCacheKeyCreator = contentCacheKeyCreator;

            if (contentEvents == null)
            {
                return;
            }

            contentEvents.PublishedContent += (s, e) => CacheManager.Remove(key: MasterKey);
            contentEvents.DeletedContent += (s, e) => CacheManager.Remove(key: MasterKey);
            contentEvents.SavedContent += (s, e) => CacheManager.Remove(key: MasterKey);
        }

        /// <summary>
        /// Gets the checked options service.
        /// </summary>
        /// <value>The checked options service.</value>
        protected CheckedOptionsService CheckedOptionsService { get; }

        /// <summary>
        /// Gets the search client.
        /// </summary>
        /// <value>The search client.</value>
        protected IClient Client { get; }

        /// <summary>
        /// Gets the content repository.
        /// </summary>
        /// <value>The content repository.</value>
        protected IContentRepository ContentRepository { get; }

        /// <summary>
        /// Gets the filtered contents with generic types.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> of <see cref="FilterContentModelType"/>.</value>
        protected IEnumerable<FilterContentModelType> FilterContentsWithGenericTypes
        {
            get
            {
                if (this.filterContentsWithGenericTypes != null)
                {
                    return this.filterContentsWithGenericTypes;
                }

                List<FilterContentModelType> filterContentModelTypes = new List<FilterContentModelType>();

                foreach (KeyValuePair<IFilterContent, FacetFilterSetting> filterContent in this.filterConfiguration
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

                this.filterContentsWithGenericTypes = filterContentModelTypes;

                return this.filterContentsWithGenericTypes;
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the reference converter.
        /// </summary>
        /// <value>The reference converter.</value>
        protected ReferenceConverter ReferenceConverter { get; }

        /// <summary>
        /// Gets the search sorting service.
        /// </summary>
        /// <value>The search sorting service.</value>
        protected SearchSortingService SearchSortingService { get; }

        /// <summary>
        /// Gets the service locator.
        /// </summary>
        /// <value>The service locator.</value>
        protected IServiceLocator ServiceLocator { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of items.</returns>
        public abstract IEnumerable<T> GetItems(ContentQueryParameters parameters);

        /// <summary>
        /// Caches the specified cache key.
        /// </summary>
        /// <typeparam name="TCache">The type of the t cache.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="result">The result.</param>
        /// <param name="contentLink">The content link.</param>
        protected virtual void Cache<TCache>(string cacheKey, TCache result, ContentReference contentLink)
            where TCache : class
        {
            this.synchronizedObjectInstanceCache.Insert(
                key: cacheKey,
                value: result,
                evictionPolicy: new CacheEvictionPolicy(
                    null,
                    new[]
                        {
                            MasterKey, this.contentCacheKeyCreator.RootKeyName, "EP:CatalogKeyPricesMasterCacheKey",
                            "Mediachase.Commerce.InventoryService.Storage$MASTER"
                        }));
        }

        /// <summary>
        /// Creates the search query.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>The search query.</returns>
        protected virtual ISearch CreateSearchQuery(Type contentType)
        {
            // TODO: Consider another way of creating an instance of the generic search. Invoke is pretty slow.
            try
            {
                MethodInfo method = typeof(Client).GetMethod(name: SearchMethodName, types: Type.EmptyTypes);
                MethodInfo genericMethod = method?.MakeGenericMethod(contentType);
                return genericMethod?.Invoke(obj: this.Client, parameters: null) as ISearch;
            }
            catch (Exception exception)
            {
                this.Logger.Log(level: Level.Error, message: exception.Message, exception: exception);
            }

            return null;
        }

        /// <summary>
        /// Gets the content of the cached.
        /// </summary>
        /// <typeparam name="TCache">The type of the t cache.</typeparam>
        /// <param name="cacheKey">The cache key.</param>
        /// <returns>The cached content.</returns>
        protected virtual TCache GetCachedContent<TCache>(string cacheKey)
            where TCache : class
        {
            return this.synchronizedObjectInstanceCache.Get(key: cacheKey) as TCache;
        }

        /// <summary>
        /// Gets the content link.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="listingMode">The listing mode.</param>
        /// <returns>A <see cref="ContentReference"/>.</returns>
        protected virtual ContentReference GetContentLink(ContentQueryParameters parameters, ListingMode listingMode)
        {
            if (parameters == null)
            {
                return this.ReferenceConverter.GetRootLink();
            }

            return listingMode == ListingMode.WidgetListing ? this.ReferenceConverter.GetRootLink() : parameters.ReferenceId;
        }

        /// <summary>
        /// Gets the listing mode.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The <see cref="ListingMode"/>.</returns>
        protected virtual ListingMode GetListingMode(ContentQueryParameters parameters)
        {
            if (parameters == null)
            {
                return ListingMode.NoListing;
            }

            string listingModeString = parameters.AllParameters["listingMode"];
            ListingMode listingMode;

            if (listingModeString != null && Enum.TryParse(value: listingModeString, result: out listingMode))
            {
                return listingMode;
            }

            return ListingMode.NoListing;
        }

        /// <summary>
        /// Gets the type of the search.
        /// </summary>
        /// <param name="filterModel">The filter model.</param>
        /// <param name="restrictedSearchType">Type of the restricted search.</param>
        /// <returns>The type to search for.</returns>
        protected virtual Type GetSearchType(FilterModel filterModel, Type restrictedSearchType)
        {
            if (filterModel == null)
            {
                return restrictedSearchType;
            }

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

        /// <summary>
        /// Gets the supported filter content model types.
        /// </summary>
        /// <param name="queryType">Type of the query.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="FilterContentModelType"/>.</returns>
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

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <param name="filterContentType">Type of the filter content.</param>
        /// <returns>The content type.</returns>
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

        /// <summary>
        /// Class FilterContentModelType.
        /// </summary>
        protected class FilterContentModelType
        {
            /// <summary>
            /// Gets or sets the type of the content.
            /// </summary>
            /// <value>The type of the content.</value>
            public Type ContentType { get; set; }

            /// <summary>
            /// Gets or sets the filter.
            /// </summary>
            /// <value>The filter.</value>
            public IFilterContent Filter { get; set; }

            // public bool FacetAdded { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this instance has a generic argument.
            /// </summary>
            /// <value><c>true</c> if this instance has a generic argument; otherwise, <c>false</c>.</value>
            public bool HasGenericArgument { get; set; }

            /// <summary>
            /// Gets or sets the setting.
            /// </summary>
            /// <value>The setting.</value>
            public FacetFilterSetting Setting { get; set; }
        }
    }
}