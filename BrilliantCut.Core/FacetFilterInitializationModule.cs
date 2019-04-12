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
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Initialization module for Find against Commerce.
    /// Implements the <see cref="EPiServer.ServiceLocation.IConfigurableModule" />
    /// </summary>
    /// <seealso cref="EPiServer.ServiceLocation.IConfigurableModule" />
    [InitializableModule]
    [ModuleDependency(typeof(IndexingModule))]
    public class FacetFilterInitializationModule : IConfigurableModule
    {
        /// <summary>
        /// The client
        /// </summary>
        private IClient client;

        /// <summary>
        /// Configure the IoC container before initialization.
        /// </summary>
        /// <param name="context">The context on which the container can be accessed.</param>
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
        }

        /// <summary>
        /// Sets client conventions in Find against Commerce, and register unified search for products.
        /// </summary>
        /// <param name="context">The initialization engine.</param>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there is are errors resolving the service instance.</exception>
        /// <remarks>Gets called as part of the EPiServer Framework initialization sequence. Note that it will be called
        /// only once per AppDomain, unless the method throws an exception. If an exception is thrown, the initialization
        /// method will be called repeatedly for each request reaching the site until the method succeeds.</remarks>
        public void Initialize(InitializationEngine context)
        {
            if (context == null)
            {
                return;
            }

            this.client = context.Locate.Advanced.GetInstance<IClient>();
            this.SetClientConventions();
        }

        /// <summary>
        /// Preloads the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        /// Unregister products from unified search.
        /// </summary>
        /// <param name="context">The initialization engine.</param>
        /// <remarks><para>
        /// This method is usually not called when running under a web application since the web app may be shut down very
        /// abruptly, but your module should still implement it properly since it will make integration and unit testing
        /// much simpler.
        /// </para>
        /// <para>
        /// Any work done by <see cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)" /> as well as any code executing on <see cref="E:EPiServer.Framework.Initialization.InitializationEngine.InitComplete" /> should be reversed.
        /// </para></remarks>
        public void Uninitialize(InitializationEngine context)
        {
        }

        /// <summary>
        /// Sets the client conventions.
        /// </summary>
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