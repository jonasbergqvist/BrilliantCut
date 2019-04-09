// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SliderFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using BrilliantCut.Core.FilterSettings;

    public sealed class SliderFilterAttribute : FacetFilterAttribute
    {
        public SliderFilterAttribute()
            : this(0, 100)
        {
        }

        public SliderFilterAttribute(int min, int max)
        {
            this.Setting = new SliderFilterSetting();
        }
    }
}