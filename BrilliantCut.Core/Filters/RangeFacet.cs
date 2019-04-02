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

    [SliderFilter]
    public class RangeFacet<TContent, TValue> : FacetBase<TContent, TValue>
        where TContent : IContent
    {
        public Func<FilterBuilder<TContent>, IEnumerable<TValue>, FilterBuilder<TContent>> FilterBuilder { get; set; }

        public override ITypeSearch<TContent> AddFacetToQuery(ITypeSearch<TContent> query, FacetFilterSetting setting)
        {
            return query.StatisticalFacetFor(fieldSelector: this.PropertyValuesExpressionObject);
        }

        public override ITypeSearch<TContent> Filter(
            IContent currentCntent,
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

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            StatisticalFacet facet =
                searchResults.StatisticalFacetFor(fieldSelector: this.PropertyValuesExpressionObject);

            const int defaultMin = 0;
            const int defaultMax = 100;

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