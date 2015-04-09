using System.Collections.Generic;

namespace BrilliantCut.Core.Models
{
    public class FilterModel
    {
        public bool ProductGrouped { get; set; }
        public IDictionary<string, List<object>> CheckedItems { get; set; }
    }
}
