using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using BrilliantCut.Core;
using BrilliantCut.Core.DataAnnotation;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;

namespace BrilliantCut.Core.Filters
{
    [SliderFilter]
    public class RangeFacet<TContent, TValue> : FacetBase<TContent, TValue>
        where TContent : IContent
    {
        public Func<FilterBuilder<TContent>, IEnumerable<TValue>, FilterBuilder<TContent>> FilterBuilder { get; set; }

        public override ITypeSearch<TContent> Filter(IContent currentCntent, ITypeSearch<TContent> query, IEnumerable<TValue> values)
        {
            var selectedValueArray = values.ToArray();
            if (!selectedValueArray.Any())
            {
                return query;
            }

            var marketFilter = SearchClient.Instance.BuildFilter<TContent>();
            marketFilter = FilterBuilder(marketFilter, selectedValueArray);

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode, IContent currentContent)
        {
            var facet = searchResults
                .StatisticalFacetFor(PropertyValuesExpressionObject);

            const int defaultMin = 0;
            const int defaultMax = 100;

            var min = facet.Count > 0 ? facet.Min : defaultMin;
            var max = facet.Count > 0 ? facet.Max : defaultMax;

            yield return new FilterOptionModel(Name + "min", "min", min, defaultMin, -1);
            yield return new FilterOptionModel(Name + "max", "max", max, defaultMax, -1);
        }

        public override ITypeSearch<TContent> AddfacetToQuery(ITypeSearch<TContent> query, FacetFilterSetting setting)
        {
            return query.StatisticalFacetFor(PropertyValuesExpressionObject);
        }
    }
}