
using EPiTube.FasetFilter.Core.Settings;

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
            Setting = new SliderFilterSetting();
        }
    }
}
