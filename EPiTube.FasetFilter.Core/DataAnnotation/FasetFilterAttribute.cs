using System;

namespace EPiTube.FasetFilter.Core.DataAnnotation
{
    public class FasetFilterAttribute : Attribute
    {
        public string FilterPath { get; set; }
        public string Markup { get; set; }
    }
}
