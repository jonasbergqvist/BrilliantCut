using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiTube.FasetFilter.Core.DataAnnotation;

namespace EPiTube.FasetFilter.Core.Filters
{
    [CheckboxFilter]
    public class TermsFacet<T> : FasetBase<T, string>
        where T : IContent
    {
        public Func<FilterBuilder<T>, string, FilterBuilder<T>> Aggregate { get; set; }

        public override ITypeSearch<T> Filter(IContent currentCntent, ITypeSearch<T> query, IEnumerable<string> values)
        {
            var marketFilter = SearchClient.Instance.BuildFilter<T>();
            marketFilter = values.Aggregate(marketFilter, Aggregate);

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        {
            var authorCounts = searchResults
                .TermsFacetFor(PropertyValuesExpressionObject).Terms;

            return authorCounts.Select(authorCount => new FilterOptionModel(Name + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count));
        }

        public override ITypeSearch<T> AddFasetToQuery(ITypeSearch<T> query)
        {
            var converted = Expression.Convert(PropertyValuesExpression.Body, typeof(string));

            var expression = Expression.Lambda<Func<T, string>>(converted, PropertyValuesExpression.Parameters);
            return query.TermsFacetFor(expression);
        }
    }
}
