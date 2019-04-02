// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SliderFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    public class SliderFilterSetting : FacetFilterSetting
    {
        public SliderFilterSetting()
            : this(0, 100)
        {
        }

        public SliderFilterSetting(int min, int max)
        {
            this.FilterPath = "brilliantcut/widget/SliderfacetFilter";
            this.Min = min;
            this.Max = max;
        }

        public int Max { get; set; }

        public int Min { get; set; }
    }
}