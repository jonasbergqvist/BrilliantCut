// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadiobuttonFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    /// <summary>
    /// Class RadiobuttonFilterSetting.
    /// Implements the <see cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    public class RadiobuttonFilterSetting : FacetFilterSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadiobuttonFilterSetting"/> class.
        /// </summary>
        public RadiobuttonFilterSetting()
        {
            this.FilterPath = "brilliantcut/widget/RadiobuttonfacetFilter";
        }
    }
}