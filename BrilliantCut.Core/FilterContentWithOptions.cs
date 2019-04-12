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

    /// <summary>
    /// Class FilterContentWithOptions.
    /// </summary>
    public class FilterContentWithOptions
    {
        /// <summary>
        /// The settings
        /// </summary>
        private FacetFilterSetting settings;

        // [JsonIgnore]
        // public IFilterContent FilterContent { get; set; }

        /// <summary>
        /// Gets or sets the type of the filter content.
        /// </summary>
        /// <value>The type of the filter content.</value>
        public Type FilterContentType { get; set; }

        /// <summary>
        /// Gets or sets the filter options.
        /// </summary>
        /// <value>The filter options.</value>
        public IEnumerable<IFilterOptionModel> FilterOptions { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        public FacetFilterSetting Settings
        {
            get
            {
                if (this.settings != null && !string.IsNullOrEmpty(value: this.settings.FilterPath))
                {
                    return this.settings;
                }

                int sortOrder = this.settings != null ? this.settings.SortOrder : 0;
                FacetFilterAttribute attribute = this.FilterContentType
                    .GetCustomAttributes(typeof(FacetFilterAttribute), true).OfType<FacetFilterAttribute>()
                    .FirstOrDefault();

                FacetFilterSetting facetFilterSetting = attribute != null ? attribute.Setting : new CheckboxFilterSetting();

                if (facetFilterSetting.SortOrder < 1)
                {
                    facetFilterSetting.SortOrder = sortOrder;
                }

                this.settings = facetFilterSetting;
                return this.settings;
            }

            set
            {
                this.settings = value;
            }
        }
    }
}