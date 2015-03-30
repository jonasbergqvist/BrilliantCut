
using EPiTube.FacetFilter.Core.FilterSettings;

namespace EPiTube.facetFilter.Core.DataAnnotation
{
    public class TextboxFilterAttribute : FacetFilterAttribute
    {
        public TextboxFilterAttribute()
            : this(1000)
        {
        }

        public TextboxFilterAttribute(int delay)
        {
            Setting = new TextboxFilterSetting();
        }
    }
}
