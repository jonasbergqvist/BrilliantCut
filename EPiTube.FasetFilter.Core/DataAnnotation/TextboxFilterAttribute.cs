
using EPiTube.FasetFilter.Core.Settings;

namespace EPiTube.FasetFilter.Core.DataAnnotation
{
    public class TextboxFilterAttribute : FasetFilterAttribute
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
