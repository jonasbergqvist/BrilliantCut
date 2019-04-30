// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using System;

    using BrilliantCut.Core.FilterSettings;

    /// <summary>
    /// Class FacetFilterAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public class FacetFilterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the setting.
        /// </summary>
        /// <value>The setting.</value>
        public FacetFilterSetting Setting { get; set; }
    }
}