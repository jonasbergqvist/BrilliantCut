// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterCatalogListView.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.UIDescriptors.ViewConfigurations
{
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.ServiceLocation;
    using EPiServer.Shell;

    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class FilterCatalogListView : ViewConfiguration<RootContent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCatalogListView"/> class.
        /// </summary>
        public FilterCatalogListView()
        {
            this.Key = "cataloglist";
            this.LanguagePath = "/commerce/contentediting/views/cataloglist";
            this.ControllerType = "brilliantcut/widget/FilterCataloglist";
            this.IconClass = "epi-iconList";
            this.SortOrder = 10;
            this.Category = "catalog";
        }
    }
}