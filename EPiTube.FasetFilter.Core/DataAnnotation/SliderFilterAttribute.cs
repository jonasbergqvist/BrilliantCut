
namespace EPiTube.FasetFilter.Core.DataAnnotation
{
    public class SliderFilterAttribute : FasetFilterAttribute
    {
        public SliderFilterAttribute()
            : this(0, 100)
        {
            
        }

        public SliderFilterAttribute(int min, int max)
        {
            FilterPath = "epitubefasetfilter/widget/SliderFasetFilter";
            Min = min;
            Max = max;
        }

        public int Min { get; set; }

        public int Max { get; set; }
    }
}
