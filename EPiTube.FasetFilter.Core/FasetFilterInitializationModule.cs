using System;
using System.Runtime.CompilerServices;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.ClientConventions;
using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Module;
using EPiServer.Find.Framework;
using EPiServer.Find.UnifiedSearch;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;

namespace EPiTube.FasetFilter.Core
{
    /// <summary>
    /// Initialization module for Find against Commerce.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(IndexingModule))]
    public class FasetFilterInitializationModule : IConfigurableModule
    {
        private static bool _isClientInitialized;

        #region IInitializableModule

        /// <summary>
        /// Sets client conventions in Find against Commerce, and register unified search for products.
        /// </summary>
        /// <param name="context">The initialization engine.</param>
        public void Initialize(InitializationEngine context)
        {
            ClientConventions(SearchClient.Instance);
            //RegisterUnifiedSearch(SearchClient.Instance);
        }

        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        /// Unregister products from unified search.
        /// </summary>
        /// <param name="context">The initialization engine.</param>
        public void Uninitialize(InitializationEngine context)
        {
            UnregisterUnifiedSearch(SearchClient.Instance);
        }

        #endregion

        #region UnifiedSearch

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void RegisterUnifiedSearch(IClient client)
        {
            try
            {
                if (!_isClientInitialized)
                {
                    client.Conventions.UnifiedSearchRegistry
                        .Add<CatalogContentBase>()
                        .PublicSearchFilter(x => x
                            .BuildFilter<CatalogContentBase>()
                                .FilterForVisitor() // Will only show products the user should have access to.
                                .ExcludeContainerPages()) // Only show products with a template.
                        .CustomizeProjection(x => x
                            .ProjectUrlFrom<CatalogContentBase>(p =>
                                GetVirtualPath(p.ContentLink, p.Language.Name)) // Get the Virtual path
                                .ProjectOriginalObjectGetterFrom<CatalogContentBase>(p => () =>
                                    GetProduct(new ContentReference(p.ContentLink.ID, p.ContentLink.ProviderName)))); // The original product
                }
            }
            finally
            {
                _isClientInitialized = true;
            }
        }

        private static CatalogContentBase GetProduct(ContentReference contentLink)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            return contentRepository.Get<CatalogContentBase>(contentLink);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void UnregisterUnifiedSearch(IClient client)
        {
            try
            {
                if (_isClientInitialized)
                {
                    client.Conventions.UnifiedSearchRegistry.Remove<CatalogContentBase>();
                }
            }
            finally
            {
                _isClientInitialized = false;
            }
        }

        private static string GetVirtualPath(ContentReference contentLink, string language)
        {
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var languageSelectorFactory = ServiceLocator.Current.GetInstance<LanguageSelectorFactory>();
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var product = contentLoader.Get<CatalogContentBase>(contentLink, languageSelectorFactory.Create(language));

            return urlResolver.GetUrl(product.ContentLink, language);
        }

        #endregion

        #region ClientConventions

        private static void ClientConventions(IClient client)
        {
            client.Conventions.ForInstancesOf<Price>()
                .ExcludeField(x => x.EntryContent);

            client.Conventions.ForInstancesOf<CatalogContentBase>()
                .IncludeField(x => x.ProductLinks())
                .IncludeField(x => x.VariationLinks())
                .IncludeField(x => x.ThumbnailPath())
                .IncludeField(x => x.NodeLinks())
                .IncludeField(x => x.DefaultCurrency())
                .IncludeField(x => x.LengthBase())
                .IncludeField(x => x.WeightBase())
                .IncludeField(x => x.DefaultPrice())
                .IncludeField(x => x.Prices())
                .IncludeField(x => x.Inventories())
                .IncludeField(x => x.StartPublishedNormalized());
            //    .IncludeField(x => x.LanguageName())
            //    .IncludeField(x => x.SearchTitle());

            //.IncludeField(x => x.SearchTypeName());
        }

        #endregion

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }
    }
}
