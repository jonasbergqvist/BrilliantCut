using System.Collections.Generic;
using System.Linq;
using EPiTube.FasetFilter.Core.DataAnnotation;
using EPiTube.FasetFilter.Core.Settings;

namespace EPiTube.FasetFilter.Core
{
    public class FilterContentWithOptions
    {
        private FasetFilterSetting _attribute;

        public IFilterContent FilterContent { get; set; }
        public IEnumerable<IFilterOptionModel> FilterOptions { get; set; }

        public FasetFilterSetting Attribute
        {
            get
            {
                if (_attribute != null)
                {
                    return _attribute;
                }

                var attribute = FilterContent.GetType().GetCustomAttributes(typeof (FasetFilterAttribute), true).OfType<FasetFilterAttribute>().FirstOrDefault();
                _attribute = attribute != null ? attribute.Setting : new CheckboxFilterSetting();

                return _attribute;
            }
            set { _attribute = value; }
        }
    }
}
