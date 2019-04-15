// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterContentBase.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Core;
    using EPiServer.Find;

    /// <summary>
    /// Class FilterContentBase.
    /// Implements the <see cref="BrilliantCut.Core.Filters.IFilterContent" />
    /// </summary>
    /// <typeparam name="TContentData">The type of the t content data.</typeparam>
    /// <typeparam name="TValueType">The type of the t value type.</typeparam>
    /// <seealso cref="IFilterContent" />
    public abstract class FilterContentBase<TContentData, TValueType> : IFilterContent
        where TContentData : IContent
    {
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public virtual string Description
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Adds the facet to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The <see cref="ITypeSearch{TContentData}" /> of <see cref="IContent"/>.</returns>
        public abstract ITypeSearch<TContentData> AddFacetToQuery(
            ITypeSearch<TContentData> query,
            FacetFilterSetting setting);

        /// <summary>
        /// Adds the facet to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The facet query.</returns>
        public ISearch AddFacetToQuery(ISearch query, FacetFilterSetting setting)
        {
            return this.AddFacetToQuery((ITypeSearch<TContentData>)query, setting: setting);
        }

        /// <summary>
        /// Filters the specified current content.
        /// </summary>
        /// <param name="currentContent">The current content.</param>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="ITypeSearch{TSource}" /> of <see cref="IContent"/>.</returns>
        public abstract ITypeSearch<TContentData> Filter(
            IContent currentContent,
            ITypeSearch<TContentData> query,
            IEnumerable<TValueType> values);

        /// <summary>
        /// Filters the specified current content.
        /// </summary>
        /// <param name="content">The current content.</param>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <returns>The filtered search.</returns>
        public ISearch Filter(IContent content, ISearch query, IEnumerable<object> values)
        {
            return this.Filter(
                currentContent: content,
                query: (ITypeSearch<TContentData>)query,
                values: values.Select(x => Convert.ChangeType(value: x, conversionType: typeof(TValueType), provider: CultureInfo.InvariantCulture)).Cast<TValueType>());
        }

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <param name="searchResults">The search results.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="currentContent">The current content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IFilterOptionModel"/>.</returns>
        public abstract IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent);
    }
}