using System;
using BrilliantCut.Core.FilterSettings;

namespace BrilliantCut.Core.DataAnnotation
{
    public class FacetFilterAttribute : Attribute
    {
        public FacetFilterSetting Setting { get; set; }
    }
}
