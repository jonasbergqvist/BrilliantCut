// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetFilteredChildrenQuery.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Rest.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;

    using BrilliantCut.Core.Service;

    using EPiServer;
    using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Commerce.Catalog.Provider;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Framework;
    using EPiServer.ServiceLocation;
    using EPiServer.Shell.ContentQuery;
    using EPiServer.Shell.Rest;

    using Mediachase.Commerce.Catalog;

    /// <summary>
    /// Class GetFilteredChildrenQuery.
    /// Implements the <see cref="EPiServer.Cms.Shell.UI.Rest.ContentQuery.GetChildrenQuery" />
    /// </summary>
    /// <seealso cref="EPiServer.Cms.Shell.UI.Rest.ContentQuery.GetChildrenQuery" />
    [ServiceConfiguration(typeof(IContentQuery))]
    public class GetFilteredChildrenQuery : GetChildrenQuery
    {
        /// <summary>
        /// The last connection check
        /// </summary>
        private static DateTime lastConnectionCheck = DateTime.Now;

        /// <summary>
        /// The content provider manager
        /// </summary>
        private readonly IContentProviderManager contentProviderManager;

        /// <summary>
        /// The filter content factory
        /// </summary>
        private readonly ContentFilterService filterContentFactory;

        /// <summary>
        /// The reference converter
        /// </summary>
        private readonly ReferenceConverter referenceConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFilteredChildrenQuery" /> class.
        /// </summary>
        /// <param name="contentProviderManager">The content provider manager.</param>
        /// <param name="contentRepository">The content repository.</param>
        /// <param name="filterContentFactory">The filter content factory.</param>
        /// <param name="queryHelper">The query helper.</param>
        /// <param name="languageSelectorFactory">The language selector factory.</param>
        /// <param name="referenceConverter">The reference converter.</param>
        public GetFilteredChildrenQuery(
            IContentProviderManager contentProviderManager,
            IContentRepository contentRepository,
            ContentFilterService filterContentFactory,
            IContentQueryHelper queryHelper,
            LanguageSelectorFactory languageSelectorFactory,
            ReferenceConverter referenceConverter)
            : base(
                queryHelper: queryHelper,
                contentRepository: contentRepository,
                languageSelectorFactory: languageSelectorFactory)
        {
            this.contentProviderManager = contentProviderManager;
            this.filterContentFactory = filterContentFactory;
            this.referenceConverter = referenceConverter;
        }

        /// <summary>
        /// Gets the rank.
        /// </summary>
        /// <value>The rank.</value>
        public override int Rank
        {
            get
            {
                return 10000;
            }
        }

        /// <summary>
        /// Determines whether this instance [can handle query] the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns><c>true</c> if this instance [can handle query] the specified parameters; otherwise, <c>false</c>.</returns>
        public override bool CanHandleQuery(IQueryParameters parameters)
        {
            if (!IsFindRunning())
            {
                return false;
            }

            string listingModeString = parameters.AllParameters["listingMode"];
            ListingMode listingMode;
            if (listingModeString == null || !Enum.TryParse(value: listingModeString, result: out listingMode)
                                          || listingMode == ListingMode.NoListing)
            {
                return false;
            }

            ContentQueryParameters itemQueryParam = parameters as ContentQueryParameters;
            if (itemQueryParam == null)
            {
                return false;
            }

            ContentReference referenceId = listingMode == ListingMode.WidgetListing
                                               ? this.referenceConverter.GetRootLink()
                                               : itemQueryParam.ReferenceId;

            if (ContentReference.IsNullOrEmpty(contentLink: referenceId))
            {
                return false;
            }

            CatalogContentProvider provider =
                this.contentProviderManager.ProviderMap.GetProvider(
                    contentLink: referenceId) as CatalogContentProvider;
            if (provider == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IContent"/>.</returns>
        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            try
            {
                return this.filterContentFactory.GetItems(parameters: parameters);
            }
            catch (SocketException)
            {
                lastConnectionCheck = DateTime.UtcNow.AddDays(-1);
                return Enumerable.Empty<IContent>();
            }
        }

        /// <summary>
        /// Sorts the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IContent"/>.</returns>
        protected override IEnumerable<IContent> Sort(IEnumerable<IContent> items, ContentQueryParameters parameters)
        {
            return items;
        }

        /// <summary>
        /// Determines whether [find is running].
        /// </summary>
        /// <returns><c>true</c> if [find is running]; otherwise, <c>false</c>.</returns>
        private static bool IsFindRunning()
        {
            try
            {
                if (DateTime.UtcNow - lastConnectionCheck < new TimeSpan(0, 1, 0))
                {
                    SearchClient.Instance.Search<CatalogContentBase>().Take(0);
                    lastConnectionCheck = DateTime.UtcNow;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}