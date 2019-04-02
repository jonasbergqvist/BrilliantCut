// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckedOptionsModel.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    using System.Collections.Generic;

    public class FilterModel
    {
        public IDictionary<string, List<object>> CheckedItems { get; set; }

        public bool ProductGrouped { get; set; }
    }
}