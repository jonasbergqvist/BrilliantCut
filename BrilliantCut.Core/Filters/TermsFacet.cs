// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TermsFacet.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Api.Facets;
    using EPiServer.Find.Framework;

    [CheckboxFilter]
    public class TermsFacet<T> : FacetBase<T, string>
        where T : IContent
    {
        public Func<FilterBuilder<T>, string, FilterBuilder<T>> Aggregate { get; set; }

        public override ITypeSearch<T> AddFacetToQuery(ITypeSearch<T> query, FacetFilterSetting setting)
        {
            UnaryExpression converted = Expression.Convert(
                expression: this.PropertyValuesExpression.Body,
                type: typeof(string));

            Expression<Func<T, string>> expression = Expression.Lambda<Func<T, string>>(
                body: converted,
                parameters: this.PropertyValuesExpression.Parameters);
            return query.TermsFacetFor(
                fieldSelector: expression,
                facetRequestAction: request =>
                    {
                        if (setting.MaxFacetHits.HasValue)
                        {
                            request.Size = setting.MaxFacetHits;
                        }
                    });
        }

        public override ITypeSearch<T> Filter(IContent currentCntent, ITypeSearch<T> query, IEnumerable<string> values)
        {
            FilterBuilder<T> marketFilter = SearchClient.Instance.BuildFilter<T>();
            marketFilter = values.Aggregate(seed: marketFilter, func: this.Aggregate);

            return query.Filter(filter: marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            IEnumerable<TermCount> facet = searchResults
                .TermsFacetFor(fieldSelector: this.PropertyValuesExpressionObject).Terms;

            return facet.Select(
                authorCount => new FilterOptionModel(
                    this.Name + authorCount.Term,
                    string.Format(
                        provider: CultureInfo.InvariantCulture,
                        format: "{0} ({1})",
                        arg0: authorCount.Term,
                        arg1: authorCount.Count),
                    value: authorCount.Term,
                    defaultValue: false,
                    count: authorCount.Count));
        }
    }
}