// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SliderFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    /// <summary>
    /// Class SliderFilterSetting.
    /// Implements the <see cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    public class SliderFilterSetting : FacetFilterSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderFilterSetting"/> class.
        /// </summary>
        public SliderFilterSetting()
            : this(0, 100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SliderFilterSetting"/> class.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public SliderFilterSetting(int min, int max)
        {
            this.FilterPath = "brilliantcut/widget/SliderfacetFilter";
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Gets or sets the maximum of the parameters.
        /// </summary>
        /// <value>The maximum.</value>
        public int Max { get; set; }

        /// <summary>
        /// Gets or sets the minimum of the parameters.
        /// </summary>
        /// <value>The minimum.</value>
        public int Min { get; set; }
    }
}