using System;
using EPiTube.FacetFilter.Core.FilterSettings;

namespace EPiTube.facetFilter.Core.DataAnnotation
{
    public class FacetFilterAttribute : Attribute
    {
        public FacetFilterSetting Setting { get; set; }
    }
}
