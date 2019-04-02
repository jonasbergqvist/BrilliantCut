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

    [ServiceConfiguration(typeof(IContentQuery))]
    public class GetFilteredChildrenQuery : GetChildrenQuery
    {
        private static DateTime _lastConnectionCheck = DateTime.Now;

        private readonly IContentProviderManager _contentProviderManager;

        private readonly ContentFilterService _filterContentFactory;

        private readonly ReferenceConverter _referenceConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFilteredChildrenQuery" /> class.
        /// </summary>
        /// <param name="queryHelper">The query helper.</param>
        /// <param name="contentRepository">The content repository.</param>
        /// <param name="languageSelectorFactory">The language selector factory.</param>
        /// <param name="contentProviderManager">The content provider manager.</param>
        /// <param name="filterContentFactory"></param>
        /// <param name="referenceConverter"></param>
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
            this._contentProviderManager = contentProviderManager;
            this._filterContentFactory = filterContentFactory;
            this._referenceConverter = referenceConverter;
        }

        public override int Rank
        {
            get
            {
                return 10000;
            }
        }

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
                                               ? this._referenceConverter.GetRootLink()
                                               : itemQueryParam.ReferenceId;

            if (ContentReference.IsNullOrEmpty(contentLink: referenceId))
            {
                return false;
            }

            CatalogContentProvider provider =
                this._contentProviderManager.ProviderMap.GetProvider(
                    contentLink: referenceId) as CatalogContentProvider;
            if (provider == null)
            {
                return false;
            }

            return true;
        }

        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            try
            {
                return this._filterContentFactory.GetItems(parameters: parameters);
            }
            catch (SocketException)
            {
                _lastConnectionCheck = DateTime.UtcNow.AddDays(-1);
                return Enumerable.Empty<IContent>();
            }
        }

        protected override IEnumerable<IContent> Sort(IEnumerable<IContent> items, ContentQueryParameters parameters)
        {
            return items;
        }

        private static bool IsFindRunning()
        {
            try
            {
                if (DateTime.UtcNow - _lastConnectionCheck < new TimeSpan(0, 1, 0))
                {
                    SearchClient.Instance.Search<CatalogContentBase>().Take(0);
                    _lastConnectionCheck = DateTime.UtcNow;
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