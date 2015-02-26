using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiTube.FasetFilter.Core.DataAnnotation;

namespace EPiTube.FasetFilter.Core
{
    public class FilterContentWithOptions
    {
        private FasetFilterAttribute _attribute;

        public IFilterContent FilterContent { get; set; }
        public IEnumerable<KeyValuePair<string, object>> FilterOptions { get; set; }

        public FasetFilterAttribute Attribute
        {
            get
            {
                if (_attribute != null)
                {
                    return _attribute;
                }

                var attribute = FilterContent.GetType().GetCustomAttributes(typeof (FasetFilterAttribute), true).OfType<FasetFilterAttribute>().FirstOrDefault();
                _attribute = attribute ?? new CheckboxFilterAttribute();

                return _attribute;
            }
        }
    }
}
