using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Find;
using EPiServer.Find.ClientConventions;
using EPiServer.Find.Cms;
using EPiServer.Find.Cms.Module;
using EPiServer.Find.Framework;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiTube.FacetFilter.Core.Extensions;

namespace EPiTube.facetFilter.Core
{
    /// <summary>
    /// Initialization module for Find against Commerce.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(IndexingModule))]
    public class facetFilterInitializationModule : IConfigurableModule
    {
        #region IInitializableModule

        /// <summary>
        /// Sets client conventions in Find against Commerce, and register unified search for products.
        /// </summary>
        /// <param name="context">The initialization engine.</param>
        public void Initialize(InitializationEngine context)
        {
            ClientConventions(SearchClient.Instance);
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
                .IncludeField(x => x.LanguageName())
                .IncludeField(x => x.StartPublishedNormalized());

            client.Conventions.ForInstancesOf<EntryContentBase>()
                .IncludeField(x => x.SelectedMarkets());

            client.Conventions.ForInstancesOf<VariationContent>()
                .IncludeField(x => x.TotalInStock());
        }

        #endregion

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }
    }
}
