// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextboxFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    /// <summary>
    /// Class TextboxFilterSetting.
    /// Implements the <see cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    public class TextboxFilterSetting : FacetFilterSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextboxFilterSetting"/> class.
        /// </summary>
        public TextboxFilterSetting()
            : this(1000)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextboxFilterSetting"/> class.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public TextboxFilterSetting(int delay)
        {
            this.FilterPath = "brilliantcut/widget/TextboxfacetFilter";
            this.Delay = delay;
        }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public int Delay { get; set; }
    }
}