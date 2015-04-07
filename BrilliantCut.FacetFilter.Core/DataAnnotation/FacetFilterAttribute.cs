using System;
using BrilliantCut.FacetFilter.Core.FilterSettings;

namespace BrilliantCut.FacetFilter.Core.DataAnnotation
{
    public class FacetFilterAttribute : Attribute
    {
        public FacetFilterSetting Setting { get; set; }
    }
}
