
using EPiTube.FacetFilter.Core.FilterSettings;

namespace EPiTube.facetFilter.Core.DataAnnotation
{
    public class CheckboxFilterAttribute : facetFilterAttribute
    {
        public CheckboxFilterAttribute()
        {
            Setting = new CheckboxFilterSetting();
        }
    }
}
