// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckboxFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using BrilliantCut.Core.FilterSettings;

    public class CheckboxFilterAttribute : FacetFilterAttribute
    {
        public CheckboxFilterAttribute()
        {
            this.Setting = new CheckboxFilterSetting();
        }
    }
}