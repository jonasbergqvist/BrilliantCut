
using EPiTube.FacetFilter.Core.FilterSettings;

namespace EPiTube.facetFilter.Core.DataAnnotation
{
    public class CheckboxFilterAttribute : FacetFilterAttribute
    {
        public CheckboxFilterAttribute()
        {
            Setting = new CheckboxFilterSetting();
        }
    }
}
