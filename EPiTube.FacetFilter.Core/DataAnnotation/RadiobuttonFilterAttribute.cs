
using EPiTube.FacetFilter.Core.FilterSettings;

namespace EPiTube.facetFilter.Core.DataAnnotation
{
    public class RadiobuttonFilterAttribute : facetFilterAttribute
    {
        public RadiobuttonFilterAttribute()
        {
            Setting = new RadiobuttonFilterSetting();
        }
    }
}
