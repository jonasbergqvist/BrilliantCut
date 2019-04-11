// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Api.Facets;
    using EPiServer.Find.Framework;

    /// <summary>
    /// Class CategoryFilter.
    /// Implements the <see cref="FilterContentBase{TContentData,TValueType}" />
    /// </summary>
    /// <seealso cref="FilterContentBase{TContentData,TValueType}" />
    public class CategoryFilter : FilterContentBase<CatalogContentBase, string>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return "Category";
            }
        }

        /// <summary>
        /// Adds the facet to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The <see cref="T:EPiServer.Find.ITypeSearch`1" /> of <see cref="T:EPiServer.Core.IContent" />.</returns>
        public override ITypeSearch<CatalogContentBase> AddFacetToQuery(
            ITypeSearch<CatalogContentBase> query,
            FacetFilterSetting setting)
        {
            return query.TermsFacetFor(
                x => x.CategoryNames(),
                request =>
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
        /// <returns>The <see cref="ITypeSearch{TSource}" /> of <see cref="CatalogContentBase"/>.</returns>
        public override ITypeSearch<CatalogContentBase> Filter(
            IContent currentContent,
            ITypeSearch<CatalogContentBase> query,
            IEnumerable<string> values)
        {
            FilterBuilder<EntryContentBase> filter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            filter = values.Aggregate(
                seed: filter,
                func: (current, value) => current.Or(x => x.CategoryNames().MatchCaseInsensitive(value)));

            return query.Filter(filter: filter);
        }

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <param name="searchResults">The search results.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="currentContent">Content of the current.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:BrilliantCut.Core.Models.IFilterOptionModel" />.</returns>
        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            IEnumerable<TermCount> facet = searchResults.TermsFacetFor<CatalogContentBase>(x => x.CategoryNames()).Terms;

            return facet.Select(
                authorCount => new FilterOptionModel(
                    "category" + authorCount.Term,
                    string.Format(provider: CultureInfo.InvariantCulture, format: "{0} ({1})", arg0: authorCount.Term, arg1: authorCount.Count),
                    value: authorCount.Term,
                    defaultValue: false,
                    count: authorCount.Count));
        }
    }
}