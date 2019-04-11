// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckboxFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using BrilliantCut.Core.FilterSettings;

    /// <summary>
    /// Class CheckboxFilterAttribute. This class cannot be inherited.
    /// Implements the <see cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    public sealed class CheckboxFilterAttribute : FacetFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckboxFilterAttribute"/> class.
        /// </summary>
        public CheckboxFilterAttribute()
        {
            this.Setting = new CheckboxFilterSetting();
        }
    }
}