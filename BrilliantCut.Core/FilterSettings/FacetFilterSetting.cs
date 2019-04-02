// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    public class FacetFilterSetting
    {
        public string FilterPath { get; set; }

        public string Markup { get; set; }

        public int? MaxFacetHits { get; set; }

        public int SortOrder { get; set; }
    }
}