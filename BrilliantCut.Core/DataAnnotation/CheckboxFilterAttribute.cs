
using BrilliantCut.Core.FilterSettings;

namespace BrilliantCut.Core.DataAnnotation
{
    public class CheckboxFilterAttribute : FacetFilterAttribute
    {
        public CheckboxFilterAttribute()
        {
            Setting = new CheckboxFilterSetting();
        }
    }
}
