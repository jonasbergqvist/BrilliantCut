
using BrilliantCut.FacetFilter.Core.FilterSettings;

namespace BrilliantCut.FacetFilter.Core.DataAnnotation
{
    public class SliderFilterAttribute : FacetFilterAttribute
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
