// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextboxFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using BrilliantCut.Core.FilterSettings;

    public class TextboxFilterAttribute : FacetFilterAttribute
    {
        public TextboxFilterAttribute()
            : this(1000)
        {
        }

        public TextboxFilterAttribute(int delay)
        {
            this.Setting = new TextboxFilterSetting();
        }
    }
}