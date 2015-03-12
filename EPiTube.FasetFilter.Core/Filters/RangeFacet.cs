using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiTube.FasetFilter.Core.DataAnnotation;

namespace EPiTube.FasetFilter.Core.Filters
{
    [SliderFilter]
    public class RangeFacet<T> : FasetBase<T, double>
        where T : IContent
    {
        public Func<FilterBuilder<T>, IEnumerable<double>, FilterBuilder<T>> FilterBuilder { get; set; } 

        public override ITypeSearch<T> Filter(IContent currentCntent, ITypeSearch<T> query, IEnumerable<double> values)
        {
            var selectedValueArray = values.ToArray();
            if (!selectedValueArray.Any())
            {
                return query;
            }

            var marketFilter = SearchClient.Instance.BuildFilter<T>();
            marketFilter = FilterBuilder(marketFilter, selectedValueArray);

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        {
            var authorCounts = searchResults
                .StatisticalFacetFor(PropertyValuesExpressionObject);

            const int defaultMin = 0;
            const int defaultMax = 100;

            var min = authorCounts.Count > 0 ? authorCounts.Min : defaultMin;
            var max = authorCounts.Count > 0 ? authorCounts.Max : defaultMax;

            yield return new FilterOptionModel(Name + "min", "min", min, defaultMin, -1);
            yield return new FilterOptionModel(Name + "max", "max", max, defaultMax, -1);
        }

        public override ITypeSearch<T> AddFasetToQuery(ITypeSearch<T> query)
        {
            return query.StatisticalFacetFor(PropertyValuesExpressionObject);
        }
    }
}