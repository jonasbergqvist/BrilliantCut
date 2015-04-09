using EPiServer.Shell.ViewComposition;

namespace BrilliantCut.Widget.Components
{
    [Component]
    public class FacetFilterComponent : ComponentDefinitionBase
    {
        public FacetFilterComponent()
            : base("brilliantcut.widget.facetfilter")
        {
            Title = "BrilliantCut";
            Description = "Filters the catalog";
            SortOrder = 50;
            PlugInAreas = new[] { EPiServer.Shell.PlugInArea.AssetsDefaultGroup, "/episerver/commerce/assets/defaultgroup" };
            Categories = new[] { "commerce" };

            Settings.Add(new Setting("repositoryKey", "catalog"));
        }
    }
}