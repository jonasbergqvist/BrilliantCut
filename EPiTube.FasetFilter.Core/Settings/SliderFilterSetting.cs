
namespace EPiTube.FasetFilter.Core.Settings
{
    public class SliderFilterSetting : FasetFilterSetting
    {
        public SliderFilterSetting()
            : this(0, 100)
        {
            
        }

        public SliderFilterSetting(int min, int max)
        {
            FilterPath = "epitubefasetfilter/widget/SliderFasetFilter";
            Min = min;
            Max = max;
        }

        public int Min { get; set; }

        public int Max { get; set; }
    }
}
