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

            yield return new FilterOptionModel(Name + "min", "min", authorCounts.Min, 0);
            yield return new FilterOptionModel(Name + "max", "max", authorCounts.Max, 100);
        }

        public override ITypeSearch<T> AddFasetToQuery(ITypeSearch<T> query)
        {
            return query.StatisticalFacetFor(PropertyValuesExpressionObject);
        }
    }
}