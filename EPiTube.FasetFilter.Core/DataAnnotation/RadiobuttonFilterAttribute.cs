
using EPiTube.FasetFilter.Core.Settings;

namespace EPiTube.FasetFilter.Core.DataAnnotation
{
    public class RadiobuttonFilterAttribute : FasetFilterAttribute
    {
        public RadiobuttonFilterAttribute()
        {
            Setting = new RadiobuttonFilterSetting();
        }
    }
}
