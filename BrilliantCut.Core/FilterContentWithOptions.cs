// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterContentWithOptions.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    public class FilterContentWithOptions
    {
        private FacetFilterSetting _settings;

        // [JsonIgnore]
        // public IFilterContent FilterContent { get; set; }
        public Type FilterContentType { get; set; }

        public IEnumerable<IFilterOptionModel> FilterOptions { get; set; }

        public string Name { get; set; }

        public FacetFilterSetting Settings
        {
            get
            {
                if (this._settings != null && !string.IsNullOrEmpty(value: this._settings.FilterPath))
                {
                    return this._settings;
                }

                int sortOrder = this._settings != null ? this._settings.SortOrder : 0;
                FacetFilterAttribute attribute = this.FilterContentType
                    .GetCustomAttributes(typeof(FacetFilterAttribute), true).OfType<FacetFilterAttribute>()
                    .FirstOrDefault();

                FacetFilterSetting settings = attribute != null ? attribute.Setting : new CheckboxFilterSetting();
                if (settings.SortOrder < 1)
                {
                    settings.SortOrder = sortOrder;
                }

                this._settings = settings;
                return this._settings;
            }

            set
            {
                this._settings = value;
            }
        }
    }
}