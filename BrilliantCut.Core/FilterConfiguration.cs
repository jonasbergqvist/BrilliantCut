// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterConfiguration.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using BrilliantCut.Core.Filters;
    using BrilliantCut.Core.FilterSettings;

    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Class FilterConfiguration.
    /// </summary>
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    public class FilterConfiguration
    {
        /// <summary>
        /// The filters
        /// </summary>
        private readonly Dictionary<IFilterContent, FacetFilterSetting> filters =
            new Dictionary<IFilterContent, FacetFilterSetting>();

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        public IDictionary<IFilterContent, FacetFilterSetting> Filters
        {
            get
            {
                return new Dictionary<IFilterContent, FacetFilterSetting>(dictionary: this.filters);
            }
        }

        /// <summary>
        /// Facets this instance.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <returns>The FilterConfiguration.</returns>
        public FilterConfiguration Facet<TFilter>()
            where TFilter : IFilterContent
        {
            return this.Facet<TFilter>(new FacetFilterSetting());
        }

        /// <summary>
        /// Facets the specified setting.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="setting">The setting.</param>
        /// <returns>The FilterConfiguration.</returns>
        public FilterConfiguration Facet<TFilter>(FacetFilterSetting setting)
            where TFilter : IFilterContent
        {
            TFilter filter = Activator.CreateInstance<TFilter>();
            setting.SortOrder = this.GetSortOrder(setting: setting);
            this.filters.Add(key: filter, value: setting);

            return this;
        }

        // public FilterConfiguration TermsFacet<TContent>(
        // Expression<Func<TContent, IEnumerable<string>>> property,
        // Func<FilterBuilder<TContent>, IEnumerable<string>, FilterBuilder<TContent>> aggregate)
        // where TContent : IContent
        // {
        // return TermsFacet(property, aggregate, new FacetFilterSetting());
        // }

        // public FilterConfiguration TermsFacet<TContent>(
        // Expression<Func<TContent, IEnumerable<string>>> property,
        // Func<FilterBuilder<TContent>, IEnumerable<string>, FilterBuilder<TContent>> aggregate,
        // FacetFilterSetting setting)
        // where TContent : IContent
        // {
        // var filter = Activator.CreateInstance<TermsFacet<TContent>>();

        // filter.PropertyValuesExpression = property;
        // filter.Aggregate = aggregate;

        // setting.SortOrder = GetSortOrder(setting);
        // _filters.Add(filter, setting);

        // return this;
        // }

        /// <summary>
        /// Ranges the facet.
        /// </summary>
        /// <typeparam name="TContent">The type of the t content.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="filterBuilder">The filter builder.</param>
        /// <returns>The FilterConfiguration.</returns>
        public FilterConfiguration RangeFacet<TContent, TValue>(
            Expression<Func<TContent, TValue>> property,
            Func<FilterBuilder<TContent>, IEnumerable<TValue>, FilterBuilder<TContent>> filterBuilder)
            where TContent : IContent
        {
            return this.RangeFacet(property: property, filterBuilder: filterBuilder, setting: new FacetFilterSetting());
        }

        /// <summary>
        /// Ranges the facet.
        /// </summary>
        /// <typeparam name="TContent">The type of the t content.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="filterBuilder">The filter builder.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The FilterConfiguration.</returns>
        public FilterConfiguration RangeFacet<TContent, TValue>(
            Expression<Func<TContent, TValue>> property,
            Func<FilterBuilder<TContent>, IEnumerable<TValue>, FilterBuilder<TContent>> filterBuilder,
            FacetFilterSetting setting)
            where TContent : IContent
        {
            RangeFacet<TContent, TValue> filter = Activator.CreateInstance<RangeFacet<TContent, TValue>>();

            filter.PropertyValuesExpression = property;
            filter.FilterBuilder = filterBuilder;

            setting.SortOrder = this.GetSortOrder(setting: setting);
            this.filters.Add(key: filter, value: setting);

            return this;
        }

        /// <summary>
        /// Gets terms facets for the specified property.
        /// </summary>
        /// <typeparam name="TContent">The type of the t content.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns>The FilterConfiguration.</returns>
        public FilterConfiguration TermsFacet<TContent>(
            Expression<Func<TContent, string>> property,
            Func<FilterBuilder<TContent>, string, FilterBuilder<TContent>> aggregate)
            where TContent : IContent
        {
            return this.TermsFacet(property: property, aggregate: aggregate, setting: new FacetFilterSetting());
        }

        /// <summary>
        /// Gets terms facets for the specified property.
        /// </summary>
        /// <typeparam name="TContent">The type of the t content.</typeparam>
        /// <param name="property">The property.</param>
        /// <param name="aggregate">The aggregate.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The FilterConfiguration.</returns>
        public FilterConfiguration TermsFacet<TContent>(
            Expression<Func<TContent, string>> property,
            Func<FilterBuilder<TContent>, string, FilterBuilder<TContent>> aggregate,
            FacetFilterSetting setting)
            where TContent : IContent
        {
            TermsFacet<TContent> filter = Activator.CreateInstance<TermsFacet<TContent>>();

            filter.PropertyValuesExpression = property;
            filter.Aggregate = aggregate;

            setting.SortOrder = this.GetSortOrder(setting: setting);
            this.filters.Add(key: filter, value: setting);

            return this;
        }

        /// <summary>
        /// Gets the sort order.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>The sort order.</returns>
        protected virtual int GetSortOrder(FacetFilterSetting setting)
        {
            if (setting.SortOrder > 0)
            {
                return setting.SortOrder;
            }

            return this.filters.Any() ? this.filters.Values.Select(x => x.SortOrder).Max() + 1 : 1;
        }
    }
}