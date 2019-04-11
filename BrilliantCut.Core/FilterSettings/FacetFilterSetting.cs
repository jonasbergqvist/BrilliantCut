// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetFilterSetting.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.FilterSettings
{
    /// <summary>
    /// Class FacetFilterSetting.
    /// </summary>
    public class FacetFilterSetting
    {
        /// <summary>
        /// Gets or sets the filter path.
        /// </summary>
        /// <value>The filter path.</value>
        public string FilterPath { get; set; }

        /// <summary>
        /// Gets or sets the markup.
        /// </summary>
        /// <value>The markup.</value>
        public string Markup { get; set; }

        /// <summary>
        /// Gets or sets the maximum facet hits.
        /// </summary>
        /// <value>The maximum facet hits.</value>
        public int? MaxFacetHits { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }
    }
}