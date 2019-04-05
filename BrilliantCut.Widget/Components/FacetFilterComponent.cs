// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetFilterComponent.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Widget.Components
{
    using EPiServer.Shell.ViewComposition;

    [Component]
    public sealed class FacetFilterComponent : ComponentDefinitionBase
    {
        public FacetFilterComponent()
            : base("brilliantcut/widget/facetfilter")
        {
            this.Title = "BrilliantCut";
            this.Description = "Filters the catalog";
            this.SortOrder = 50;
            this.PlugInAreas = new[]
                                   {
                                       EPiServer.Shell.PlugInArea.AssetsDefaultGroup,
                                       "/episerver/commerce/assets/defaultgroup"
                                   };
            this.Categories = new[] { "commerce" };

            this.Settings.Add(new Setting("repositoryKey", "catalog"));
        }
    }
}