
using BrilliantCut.FacetFilter.Core.FilterSettings;

namespace BrilliantCut.FacetFilter.Core.DataAnnotation
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
