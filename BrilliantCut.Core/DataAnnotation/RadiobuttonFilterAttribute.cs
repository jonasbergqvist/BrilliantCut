
using BrilliantCut.Core.FilterSettings;

namespace BrilliantCut.Core.DataAnnotation
{
    public class RadiobuttonFilterAttribute : FacetFilterAttribute
    {
        public RadiobuttonFilterAttribute()
        {
            Setting = new RadiobuttonFilterSetting();
        }
    }
}
