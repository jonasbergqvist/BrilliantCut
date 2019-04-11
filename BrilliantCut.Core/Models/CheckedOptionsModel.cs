// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckedOptionsModel.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Class FilterModel.
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        /// Gets or sets the checked items.
        /// </summary>
        /// <value>The checked items.</value>
        public IDictionary<string, List<object>> CheckedItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [product must be grouped].
        /// </summary>
        /// <value><c>true</c> if [product must be grouped]; otherwise, <c>false</c>.</value>
        public bool ProductGrouped { get; set; }
    }
}