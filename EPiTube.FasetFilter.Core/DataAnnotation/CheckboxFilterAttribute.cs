
using EPiTube.FasetFilter.Core.Settings;

namespace EPiTube.FasetFilter.Core.DataAnnotation
{
    public class CheckboxFilterAttribute : FasetFilterAttribute
    {
        public CheckboxFilterAttribute()
        {
            Setting = new CheckboxFilterSetting();
        }
    }
}
