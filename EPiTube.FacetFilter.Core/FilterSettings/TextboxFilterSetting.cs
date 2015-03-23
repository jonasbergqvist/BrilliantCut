
namespace EPiTube.FacetFilter.Core.FilterSettings
{
    public class TextboxFilterSetting : FacetFilterSetting
    {
        public TextboxFilterSetting()
            : this(1000)
        {
        }

        public TextboxFilterSetting(int delay)
        {
            FilterPath = "epitubefacetfilter/widget/TextboxfacetFilter";
            Delay = delay;
        }

        public int Delay { get; set; }
    }
}
