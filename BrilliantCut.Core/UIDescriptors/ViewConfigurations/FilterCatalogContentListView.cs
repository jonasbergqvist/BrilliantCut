// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterCatalogContentListView.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.UIDescriptors.ViewConfigurations
{
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.ServiceLocation;
    using EPiServer.Shell;

    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class FilterCatalogContentListView : ViewConfiguration<NodeContentBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCatalogContentListView"/> class.
        /// </summary>
        public FilterCatalogContentListView()
        {
            this.Key = "catalogcontentlist";
            this.LanguagePath = "/commerce/contentediting/views/cataloglist";
            this.ControllerType = "brilliantcut/widget/FilterCatalogContentlist";
            this.IconClass = "epi-iconList";
            this.SortOrder = 10;
            this.Category = "catalog";
        }
    }
}