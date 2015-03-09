using System;
using EPiTube.FasetFilter.Core.Settings;

namespace EPiTube.FasetFilter.Core.DataAnnotation
{
    public class FasetFilterAttribute : Attribute
    {
        public FasetFilterSetting Setting { get; set; }
    }
}
