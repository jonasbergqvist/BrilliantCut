// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckboxFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    public class CheckboxFilterSetting : FacetFilterSetting
    {
        public CheckboxFilterSetting()
        {
            this.FilterPath = "brilliantcut/widget/CheckboxfacetFilter";
        }
    }
}