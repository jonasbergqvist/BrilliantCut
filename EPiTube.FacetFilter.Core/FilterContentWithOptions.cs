﻿using System;
using System.Collections.Generic;
using System.Linq;
using EPiTube.facetFilter.Core.DataAnnotation;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.Filters;
using EPiTube.FacetFilter.Core.FilterSettings;
using EPiTube.FacetFilter.Core.Models;
using Newtonsoft.Json;

namespace EPiTube.facetFilter.Core
{
    public class FilterContentWithOptions
    {
        private FacetFilterSetting _settings;

        [JsonIgnore]
        public IFilterContent FilterContent { get; set; }

        public string Name { get { return FilterContent.Name; }}

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
                var attribute = FilterContent.GetType().GetCustomAttributes(typeof (FacetFilterAttribute), true).OfType<FacetFilterAttribute>().FirstOrDefault();
                
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