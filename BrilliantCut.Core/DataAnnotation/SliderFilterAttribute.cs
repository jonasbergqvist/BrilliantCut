// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SliderFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using System;

    using BrilliantCut.Core.FilterSettings;

    /// <summary>
    /// Class SliderFilterAttribute. This class cannot be inherited.
    /// Implements the <see cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SliderFilterAttribute : FacetFilterAttribute
    {
        private readonly int min;

        private readonly int max;

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
            this.min = min;
            this.max = min;
            this.Setting = new SliderFilterSetting(min: min, max: max);
        }

        /// <summary>
        /// Gets the maximum of the parameters.
        /// </summary>
        /// <value>The maximum.</value>
        public int Max
        {
            get
            {
                return this.max;
            }
        }

        /// <summary>
        /// Gets the minimum of the parameters.
        /// </summary>
        /// <value>The minimum.</value>
        public int Min
        {
            get
            {
                return this.min;
            }
        }
    }
}