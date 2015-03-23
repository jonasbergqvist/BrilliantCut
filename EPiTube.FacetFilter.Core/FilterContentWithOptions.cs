using System.Collections.Generic;
using System.Linq;
using EPiTube.facetFilter.Core.DataAnnotation;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.Filters;
using EPiTube.FacetFilter.Core.FilterSettings;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.facetFilter.Core
{
    public class FilterContentWithOptions
    {
        private FacetFilterSetting _settings;

        public IFilterContent FilterContent { get; set; }
        public IEnumerable<IFilterOptionModel> FilterOptions { get; set; }

        public FacetFilterSetting Settings
        {
            get
            {
                if (_settings != null)
                {
                    return _settings;
                }

                var attribute = FilterContent.GetType().GetCustomAttributes(typeof (facetFilterAttribute), true).OfType<facetFilterAttribute>().FirstOrDefault();
                _settings = attribute != null ? attribute.Setting : new CheckboxFilterSetting();

                return _settings;
            }
            set { _settings = value; }
        }
    }
}
