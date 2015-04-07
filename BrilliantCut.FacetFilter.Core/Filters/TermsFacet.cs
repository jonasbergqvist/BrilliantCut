using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using BrilliantCut.FacetFilter.Core;
using BrilliantCut.FacetFilter.Core.DataAnnotation;
using BrilliantCut.FacetFilter.Core.Extensions;
using BrilliantCut.FacetFilter.Core.FilterSettings;
using BrilliantCut.FacetFilter.Core.Models;

namespace BrilliantCut.FacetFilter.Core.Filters
{
    [CheckboxFilter]
    public class TermsFacet<T> : FacetBase<T, string>
        where T : IContent
    {
        public Func<FilterBuilder<T>, string, FilterBuilder<T>> Aggregate { get; set; }

        public override ITypeSearch<T> Filter(IContent currentCntent, ITypeSearch<T> query, IEnumerable<string> values)
        {
            var marketFilter = SearchClient.Instance.BuildFilter<T>();
            marketFilter = values.Aggregate(marketFilter, Aggregate);

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode)
        {
            var facet = searchResults
                .TermsFacetFor(PropertyValuesExpressionObject).Terms;

            return facet.Select(authorCount => new FilterOptionModel(Name + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count));
        }

        public override ITypeSearch<T> AddfacetToQuery(ITypeSearch<T> query, FacetFilterSetting setting)
        {
            var converted = Expression.Convert(PropertyValuesExpression.Body, typeof(string));

            var expression = Expression.Lambda<Func<T, string>>(converted, PropertyValuesExpression.Parameters);
            return query.TermsFacetFor(expression, request =>
            {
                if (setting.MaxFacetHits.HasValue)
                {
                    request.Size = setting.MaxFacetHits;
                }
            });
        }
    }
}
