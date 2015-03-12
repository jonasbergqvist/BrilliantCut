using System.Collections.Generic;

namespace EPiTube.FasetFilter.Core
{
    public class FilterModel
    {
        public bool ProductGrouped { get; set; }
        public List<FilterContentModel> Value { get; set; }
    }

    public class FilterContentModel
    {
        public string Name { get; set; }
        public List<FilterContentOptionModel> Value { get; set; }
    }

    public class FilterContentOptionModel
    {
        public object Value { get; set; }
    }
}
