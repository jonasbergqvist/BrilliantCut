// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeFacet.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Api.Facets;
    using EPiServer.Find.Framework;

    /// <summary>
    /// Class RangeFacet.
    /// Implements the <see cref="FacetBase{TContent, TValue}" />
    /// </summary>
    /// <typeparam name="TContent">The type of the t content.</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="FacetBase{TContent, TValue}" />
    [SliderFilter]
    public class RangeFacet<TContent, TValue> : FacetBase<TContent, TValue>
        where TContent : IContent
    {
        /// <summary>
        /// Gets or sets the filter builder.
        /// </summary>
        /// <value>The filter builder.</value>
        public Func<FilterBuilder<TContent>, IEnumerable<TValue>, FilterBuilder<TContent>> FilterBuilder { get; set; }

        /// <summary>
        /// Adds the facet to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The <see cref="T:EPiServer.Find.ITypeSearch`1" /> of <see cref="T:EPiServer.Core.IContent" />.</returns>
        public override ITypeSearch<TContent> AddFacetToQuery(ITypeSearch<TContent> query, FacetFilterSetting setting)
        {
            return query.StatisticalFacetFor(fieldSelector: this.PropertyValuesExpressionObject);
        }

        /// <summary>
        /// Filters the specified current content.
        /// </summary>
        /// <param name="currentContent">The current content.</param>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="ITypeSearch{TSource}"/>;.</returns>
        public override ITypeSearch<TContent> Filter(
            IContent currentContent,
            ITypeSearch<TContent> query,
            IEnumerable<TValue> values)
        {
            TValue[] selectedValueArray = values.ToArray();

            if (!selectedValueArray.Any())
            {
                return query;
            }

            FilterBuilder<TContent> marketFilter = SearchClient.Instance.BuildFilter<TContent>();
            marketFilter = this.FilterBuilder(arg1: marketFilter, arg2: selectedValueArray);

            return query.Filter(filter: marketFilter);
        }

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <param name="searchResults">The search results.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="currentContent">The current content.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:BrilliantCut.Core.Models.IFilterOptionModel" />.</returns>
        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            StatisticalFacet facet =
                searchResults.StatisticalFacetFor(fieldSelector: this.PropertyValuesExpressionObject);

            int defaultMin = 0;
            int defaultMax = 100;

            double min = facet.Count > 0 ? facet.Min : defaultMin;
            double max = facet.Count > 0 ? facet.Max : defaultMax;

            yield return new FilterOptionModel(
                this.Name + "min",
                "min",
                value: min,
                defaultValue: defaultMin,
                count: -1);
            yield return new FilterOptionModel(
                this.Name + "max",
                "max",
                value: max,
                defaultValue: defaultMax,
                count: -1);
        }
    }
}