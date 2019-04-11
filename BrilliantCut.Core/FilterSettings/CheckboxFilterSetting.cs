// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckboxFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    /// <summary>
    /// Class CheckboxFilterSetting.
    /// Implements the <see cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.FilterSettings.FacetFilterSetting" />
    public class CheckboxFilterSetting : FacetFilterSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckboxFilterSetting"/> class.
        /// </summary>
        public CheckboxFilterSetting()
        {
            this.FilterPath = "brilliantcut/widget/CheckboxfacetFilter";
        }
    }
}