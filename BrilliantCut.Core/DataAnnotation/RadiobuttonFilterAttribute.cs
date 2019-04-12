// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadiobuttonFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using System;

    using BrilliantCut.Core.FilterSettings;

    /// <summary>
    /// Class RadiobuttonFilterAttribute. This class cannot be inherited.
    /// Implements the <see cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RadiobuttonFilterAttribute : FacetFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadiobuttonFilterAttribute"/> class.
        /// </summary>
        public RadiobuttonFilterAttribute()
        {
            this.Setting = new RadiobuttonFilterSetting();
        }
    }
}