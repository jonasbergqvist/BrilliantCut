using EPiServer.Shell.ViewComposition;

namespace EPiTube.FasetFilter.Widget.Components
{
    [Component]
    public class FasetFilterComponent : ComponentDefinitionBase
    {
        public FasetFilterComponent()
            : base("epitubefasetfilter.widget.fasetfilter")
        {
            Title = "Filters";
            Description = "Filters the catalog";
            SortOrder = 50;
            PlugInAreas = new[] { EPiServer.Shell.PlugInArea.AssetsDefaultGroup, "/episerver/commerce/assets/defaultgroup" };
            Categories = new[] { "commerce" };

            Settings.Add(new Setting("repositoryKey", "catalog"));
        }
    }
}