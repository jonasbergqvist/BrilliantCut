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

    /// <summary>
    /// Class FilterCatalogContentListView.
    /// Implements the <see cref="EPiServer.Shell.ViewConfiguration{T}" />
    /// </summary>
    /// <seealso cref="EPiServer.Shell.ViewConfiguration{T}" />
    [ServiceConfiguration(typeof(ViewConfiguration))]
    public class FilterCatalogContentListView : ViewConfiguration<NodeContentBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterCatalogContentListView" /> class.
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