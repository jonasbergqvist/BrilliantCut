using System.Collections.Generic;

namespace EPiTube.FasetFilter.Core
{
    public class FilterModel
    {
        public bool ProductGrouped { get; set; }

        //public ContentReference ContentLink { get; set; }
        public List<FilterContentModel> Value { get; set; }

        //public static FilterModel GetFilterModel()
        //{
        //    return null;
        //}
    }

    public class FilterContentModel
    {
        public string Name { get; set; }
        public List<FilterContentOptionModel> Value { get; set; }
    }

    public class FilterContentOptionModel
    {
        //public string Name { get; set; }
        public object Value { get; set; }
    }

    //public class FilterModel
    //{
    //    public ContentReference ContentLink { get; set; }
    //    public FilterContentModel[] Value { get; set; }

    //    public static FilterModel GetFilterModel()
    //    {
    //        return null;
    //    }
    //}

    //public class FilterContentModel
    //{
    //    public string Name { get; set; }
    //    public object[] Value { get; set; }
    //}
}
