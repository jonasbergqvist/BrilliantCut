// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SliderFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using BrilliantCut.Core.FilterSettings;

    /// <summary>
    /// Class SliderFilterAttribute. This class cannot be inherited.
    /// Implements the <see cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    public sealed class SliderFilterAttribute : FacetFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderFilterAttribute"/> class.
        /// </summary>
        /// <remarks>Min value = 0, max value = 100. </remarks>
        public SliderFilterAttribute()
            : this(0, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderFilterAttribute"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public SliderFilterAttribute(int min, int max)
        {
            this.Setting = new SliderFilterSetting();
        }
    }
}