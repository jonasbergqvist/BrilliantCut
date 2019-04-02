// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RadiobuttonFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using BrilliantCut.Core.FilterSettings;

    public class RadiobuttonFilterAttribute : FacetFilterAttribute
    {
        public RadiobuttonFilterAttribute()
        {
            this.Setting = new RadiobuttonFilterSetting();
        }
    }
}