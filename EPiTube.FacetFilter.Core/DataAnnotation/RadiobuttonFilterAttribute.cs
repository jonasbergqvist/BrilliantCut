
using EPiTube.FacetFilter.Core.FilterSettings;

namespace EPiTube.facetFilter.Core.DataAnnotation
{
    public class RadiobuttonFilterAttribute : FacetFilterAttribute
    {
        public RadiobuttonFilterAttribute()
        {
            Setting = new RadiobuttonFilterSetting();
        }
    }
}
