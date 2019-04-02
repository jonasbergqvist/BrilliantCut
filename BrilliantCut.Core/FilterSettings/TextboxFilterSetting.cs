// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextboxFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    public class TextboxFilterSetting : FacetFilterSetting
    {
        public TextboxFilterSetting()
            : this(1000)
        {
        }

        public TextboxFilterSetting(int delay)
        {
            this.FilterPath = "brilliantcut/widget/TextboxfacetFilter";
            this.Delay = delay;
        }

        public int Delay { get; set; }
    }
}