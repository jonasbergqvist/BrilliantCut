// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterContent.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System.Collections.Generic;

    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Core;
    using EPiServer.Find;

    /// <summary>
    /// Interface IFilterContent
    /// </summary>
    public interface IFilterContent
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Adds the facet to query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The facet search.</returns>
        ISearch AddFacetToQuery(ISearch query, FacetFilterSetting setting);

        /// <summary>
        /// Filters the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <returns>The filtered search.</returns>
        ISearch Filter(IContent content, ISearch query, IEnumerable<object> values);

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <param name="searchResults">The search results.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="currentContent">Content of the current.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IFilterOptionModel"/>.</returns>
        IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent);
    }
}