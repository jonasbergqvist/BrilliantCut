// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActiveFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Cms;

    /// <summary>
    /// Class IsActiveFilter.
    /// Implements the <see cref="FilterContentBase{TContentData,TValueType}" />
    /// </summary>
    /// <seealso cref="FilterContentBase{TContentData,TValueType}" />
    [RadiobuttonFilter]
    public class IsActiveFilter : FilterContentBase<CatalogContentBase, string>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return "Active";
            }
        }

        /// <summary>
        /// Adds the facet to query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The <see cref="ITypeSearch{TSource}" /> of <see cref="CatalogContentBase"/>.</returns>
        public override ITypeSearch<CatalogContentBase> AddFacetToQuery(
            ITypeSearch<CatalogContentBase> query,
            FacetFilterSetting setting)
        {
            return query;
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
            string value = values.FirstOrDefault();
            if (value == null || value == "all")
            {
                return query;
            }

            return query.CurrentlyPublished().ExcludeDeleted();
        }

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <param name="searchResults">The search results.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="currentContent">Content of the current.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IFilterOptionModel"/>.</returns>
        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            yield return new FilterOptionModel("all", "All", false, false, -1);
            yield return new FilterOptionModel("active", "Active", false, false, -1);

            // yield return new FilterOptionModel("unactive", "Unactive", false, false, -1);
        }
    }
}