using System;
using System.Collections.Generic;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Commerce.Catalog.Provider;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using EPiServer.Shell.Rest;

namespace EPiTube.FasetFilter.Core.Rest.Query
{
    [ServiceConfiguration(typeof(IContentQuery))]
    public class GetFilteredChildrenQuery : GetChildrenQuery
    {
        private readonly IContentProviderManager _contentProviderManager;
        private readonly FilterContentFactory _filterContentFactory;

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
            FilterContentFactory filterContentFactory,
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

            bool filterEnabled;
            var filterModelString = parameters.AllParameters["filterEnabled"];
            return filterModelString != null && Boolean.TryParse(filterModelString, out filterEnabled) && filterEnabled;
        }

        protected override IEnumerable<IContent> GetContent(ContentQueryParameters parameters)
        {
            return _filterContentFactory.GetFilteredChildren(parameters, true, false);
        }

        protected override IEnumerable<IContent> Sort(IEnumerable<IContent> items, ContentQueryParameters parameters)
        {
            return items;
        }

        //protected override ContentRange Range(IEnumerable<IContent> items, ContentQueryParameters parameters)
        //{
        //    return base.Range(items, parameters);
        //}

        //protected override IEnumerable<IContent> Filter(IEnumerable<IContent> items, ContentQueryParameters parameters)
        //{
        //    return base.Filter(items, parameters);
        //}
    }
}
