
namespace BrilliantCut.FacetFilter.Core.FilterSettings
{
    public class SliderFilterSetting : FacetFilterSetting
    {
        public SliderFilterSetting()
            : this(0, 100)
        {
            
        }

        public SliderFilterSetting(int min, int max)
        {
            FilterPath = "brilliantcut/widget/SliderfacetFilter";
            Min = min;
            Max = max;
        }

        public int Min { get; set; }

        public int Max { get; set; }
    }
}
