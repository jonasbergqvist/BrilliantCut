// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using System;

    using BrilliantCut.Core.FilterSettings;

    public class FacetFilterAttribute : Attribute
    {
        public FacetFilterSetting Setting { get; set; }
    }
}