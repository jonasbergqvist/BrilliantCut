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

    /// <summary>
    /// Class TermsFacet.
    /// Implements the <see cref="FacetBase{TContentData,TValueType}" />
    /// </summary>
    /// <typeparam name="T">The type of content</typeparam>
    /// <seealso cref="FacetBase{TContentData,TValueType}" />
    [CheckboxFilter]
    public class TermsFacet<T> : FacetBase<T, string>
        where T : IContent
    {
        /// <summary>
        /// Gets or sets the aggregate.
        /// </summary>
        /// <value>The aggregate.</value>
        public Func<FilterBuilder<T>, string, FilterBuilder<T>> Aggregate { get; set; }

        /// <summary>
        /// Adds the facet to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The <see cref="T:EPiServer.Find.ITypeSearch`1" /> of <see cref="T:EPiServer.Core.IContent" />.</returns>
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

        /// <summary>
        /// Filters the specified current content.
        /// </summary>
        /// <param name="currentContent">The current content.</param>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="ITypeSearch{TSource}"/>;.</returns>
        public override ITypeSearch<T> Filter(IContent currentContent, ITypeSearch<T> query, IEnumerable<string> values)
        {
            FilterBuilder<T> marketFilter = SearchClient.Instance.BuildFilter<T>();
            marketFilter = values.Aggregate(seed: marketFilter, func: this.Aggregate);

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
            IEnumerable<TermCount> facet = searchResults
                .TermsFacetFor(fieldSelector: this.PropertyValuesExpressionObject).Terms;

            return facet.Select(
                authorCount => new FilterOptionModel(
                    this.Name + authorCount.Term,
                    string.Format(provider: CultureInfo.InvariantCulture, format: "{0} ({1})", arg0: authorCount.Term, arg1: authorCount.Count),
                    value: authorCount.Term,
                    defaultValue: false,
                    count: authorCount.Count));
        }
    }
}