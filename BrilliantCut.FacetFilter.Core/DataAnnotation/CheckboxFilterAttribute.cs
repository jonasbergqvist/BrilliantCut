
using BrilliantCut.FacetFilter.Core.FilterSettings;

namespace BrilliantCut.FacetFilter.Core.DataAnnotation
{
    public class CheckboxFilterAttribute : FacetFilterAttribute
    {
        public CheckboxFilterAttribute()
        {
            Setting = new CheckboxFilterSetting();
        }
    }
}
