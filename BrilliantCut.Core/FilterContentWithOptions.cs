using System;
using System.Collections.Generic;
using System.Linq;
using BrilliantCut.Core.DataAnnotation;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.Filters;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;

namespace BrilliantCut.Core
{
    public class FilterContentWithOptions
    {
        private FacetFilterSetting _settings;

        //[JsonIgnore]
        //public IFilterContent FilterContent { get; set; }

        public Type FilterContentType { get; set; }

        public string Name { get; set; }

        public IEnumerable<IFilterOptionModel> FilterOptions { get; set; }

        public FacetFilterSetting Settings
        {
            get
            {
                if (_settings != null && !String.IsNullOrEmpty(_settings.FilterPath))
                {
                    return _settings;
                }

                var sortOrder = _settings != null ? _settings.SortOrder : 0;
                var attribute = FilterContentType.GetCustomAttributes(typeof(FacetFilterAttribute), true).OfType<FacetFilterAttribute>().FirstOrDefault();
                
                var settings = attribute != null ? attribute.Setting : new CheckboxFilterSetting();
                if (settings.SortOrder < 1)
                {
                    settings.SortOrder = sortOrder;
                }

                _settings = settings;
                return _settings;
            }
            set { _settings = value; }
        }
    }
}
