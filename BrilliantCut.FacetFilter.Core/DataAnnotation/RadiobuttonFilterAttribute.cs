
using BrilliantCut.FacetFilter.Core.FilterSettings;

namespace BrilliantCut.FacetFilter.Core.DataAnnotation
{
    public class RadiobuttonFilterAttribute : FacetFilterAttribute
    {
        public RadiobuttonFilterAttribute()
        {
            Setting = new RadiobuttonFilterSetting();
        }
    }
}
