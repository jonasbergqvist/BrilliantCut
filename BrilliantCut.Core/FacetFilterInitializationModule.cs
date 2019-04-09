// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetFilterInitializationModule.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core
{
    using BrilliantCut.Core.Extensions;

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
    using EPiServer.ServiceLocation.Compatibility;

    /// <summary>
    /// Initialization module for Find against Commerce.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(IndexingModule))]
    public class FacetFilterInitializationModule : IConfigurableModule
    {
        private IClient client;

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }

        /// <summary>
        /// Sets client conventions in Find against Commerce, and register unified search for products.
        /// </summary>
        /// <param name="context">The initialization engine.</param>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there is are errors resolving the service instance.</exception>
        public void Initialize(InitializationEngine context)
        {
            this.client = context.Locate.Advanced.GetInstance<IClient>();
            this.SetClientConventions();
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

        private void SetClientConventions()
        {
            this.client.Conventions.ForInstancesOf<Price>().ExcludeField(x => x.EntryContent);

            this.client.Conventions.ForInstancesOf<CatalogContentBase>().IncludeField(x => x.ParentProducts())
                .IncludeField(x => x.Variations()).IncludeField(x => x.ThumbnailUrl()).IncludeField(x => x.LinkUrl())
                .IncludeField(x => x.NodeLinks()).IncludeField(x => x.CategoryNames())
                .IncludeField(x => x.DefaultCurrency()).IncludeField(x => x.LengthBase())
                .IncludeField(x => x.WeightBase()).IncludeField(x => x.DefaultPriceValue())
                .IncludeField(x => x.Prices()).IncludeField(x => x.Inventories()).IncludeField(x => x.LanguageName())
                .IncludeField(x => x.StartPublishedNormalized());

            this.client.Conventions.ForInstancesOf<EntryContentBase>().IncludeField(x => x.SelectedMarkets());

            this.client.Conventions.ForInstancesOf<VariationContent>().IncludeField(x => x.TotalInStock());
        }
    }
}