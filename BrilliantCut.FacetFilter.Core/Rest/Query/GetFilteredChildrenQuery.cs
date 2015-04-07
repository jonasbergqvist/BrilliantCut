using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using BrilliantCut.FacetFilter.Core.Service;
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

namespace BrilliantCut.FacetFilter.Core.Rest.Query
{
    [ServiceConfiguration(typeof(IContentQuery))]
    public class GetFilteredChildrenQuery : GetChildrenQuery
    {
        private readonly IContentProviderManager _contentProviderManager;
        private readonly ContentFilterService _filterContentFactory;

        private static DateTime _lastConnectionCheck = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFilteredChildrenQuery" /> class.
        /// </summary>
        /// <param name="queryHelper">The query helper.</param>
        /// <param name="contentRepository">The content repository.</param>
        /// <param name="languageSelectorFactory">The language selector factory.</param>
        /// <param name="contentProviderManager">The content provider manager.</param>
        /// <param name="filterContentFactory"></param>
        public GetFilteredChildrenQuery(
            IContentProviderManager contentProviderManager,
            IContentRepository contentRepository,
            ContentFilterService filterContentFactory,
            IContentQueryHelper queryHelper, 
            LanguageSelectorFactory languageSelectorFactory)
            : base(queryHelper, contentRepository, languageSelectorFactory)
        {
            _contentProviderManager = contentProviderManager;
            _filterContentFactory = filterContentFactory;
        }

        public override int Rank
        {
            get { return 10000; }
        }

        public override bool CanHandleQuery(IQueryParameters parameters)
        {
            if (!IsFindRunning())
            {
                return false;
            }

            var itemQueryParam = parameters as ContentQueryParameters;
            if ((itemQueryParam == null) || ContentReference.IsNullOrEmpty(itemQueryParam.ReferenceId))
            {
                return false;
            }
            var provider = _contentProviderManager.ProviderMap.GetProvider(itemQueryParam.ReferenceId) as CatalogContentProvider;
            if (provider == null)
            {
                return false;
            }

            var listingModeString = parameters.AllParameters["listingMode"];
            ListingMode listingMode;
            return listingModeString != null && 
                Enum.TryParse(listingModeString, out listingMode) &&
                listingMode != ListingMode.NoListing;
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

        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            try
            {
                return _filterContentFactory.GetItems(parameters);
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
    }
}
