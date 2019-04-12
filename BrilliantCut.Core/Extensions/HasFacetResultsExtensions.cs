// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HasFacetResultsExtensions.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Extensions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Api.Facets;
    using EPiServer.Find.Helpers;
    using EPiServer.Find.Helpers.Reflection;

    /// <summary>
    /// Class HasFacetResultsExtensions.
    /// </summary>
    public static class HasFacetResultsExtensions
    {
        /// <summary>
        /// Gets the geo distance facet for.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="GeoDistanceFacet"/>.</returns>
        public static GeoDistanceFacet GeoDistanceFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, GeoLocation>> fieldSelector)
            where TContent : IContent
        {
            return facetsResultsContainer.GeoDistanceFacetFor(fieldSelector: fieldSelector);
        }

        /// <summary>
        /// Gets the facet with the specified name.
        /// </summary>
        /// <typeparam name="TFacet">The type of facet.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="facetName">Name of the facet.</param>
        /// <returns>The facet.</returns>
        public static TFacet GetFacetFor<TFacet>(IHasFacetResults facetsResultsContainer, string facetName)
            where TFacet : Facet
        {
            if (facetsResultsContainer == null)
            {
                return null;
            }

            Facet facet = facetsResultsContainer.Facets[name: facetName];

            if (facet.IsNull())
            {
                return null;
            }

            return facet as TFacet;
        }

        /// <summary>
        /// Gets the histogram facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="DateHistogramFacet"/>.</returns>
        public static DateHistogramFacet HistogramFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime?>> fieldSelector)
            where TContent : IContent
        {
            return facetsResultsContainer.HistogramFacetFor(fieldSelector: fieldSelector);
        }

        /// <summary>
        /// Gets the histogram facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="DateHistogramFacet"/>.</returns>
        public static DateHistogramFacet HistogramFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime>> fieldSelector)
            where TContent : IContent
        {
            return facetsResultsContainer.HistogramFacetFor(fieldSelector: fieldSelector);
        }

        /// <summary>
        /// Gets the histogram facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="DateHistogramFacet"/>.</returns>
        public static HistogramFacet HistogramFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, object>> fieldSelector)
            where TContent : IContent
        {
            return facetsResultsContainer.HistogramFacetFor(fieldSelector: fieldSelector);
        }

        /// <summary>
        /// Gets the range facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="DateRangeFacet"/>.</returns>
        public static DateRangeFacet RangeFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime?>> fieldSelector)
            where TContent : IContent
        {
            return facetsResultsContainer.RangeFacetFor(fieldSelector: fieldSelector);
        }

        /// <summary>
        /// Gets the range facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="DateRangeFacet"/>.</returns>
        public static DateRangeFacet RangeFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime>> fieldSelector)
            where TContent : IContent
        {
            return facetsResultsContainer.RangeFacetFor(fieldSelector: fieldSelector);
        }

        /// <summary>
        /// Gets the range facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="DateRangeFacet"/>.</returns>
        public static NumericRangeFacet RangeFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, object>> fieldSelector)
            where TContent : IContent
        {
            return facetsResultsContainer.RangeFacetFor(fieldSelector: fieldSelector);
        }

        /// <summary>
        /// Gets the range facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelector">The field selector.</param>
        /// <returns>A <see cref="DateRangeFacet"/>.</returns>
        public static StatisticalFacet StatisticalFacetFor<TContent>(
            this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, object>> fieldSelector)
            where TContent : IContent
        {
            return GetFacetFor<StatisticalFacet>(
                facetsResultsContainer: facetsResultsContainer,
                facetName: string.Format(provider: CultureInfo.InvariantCulture, format: "StatisticalFacet_{0}", arg0: fieldSelector.GetFieldPath()));
        }

        /// <summary>
        /// Gets the range facet for the specified field selector.
        /// </summary>
        /// <typeparam name="TContent">The type of content.</typeparam>
        /// <param name="facetsResultsContainer">The facets results container.</param>
        /// <param name="fieldSelectors">The field selectors.</param>
        /// <returns>A <see cref="StatisticalFacet"/>.</returns>
        /// <exception cref="T:System.ArgumentException">'fieldSelectors' needs to contain at least one expression.</exception>
        public static StatisticalFacet StatisticalFacetForSumOf<TContent>(
            this IHasFacetResults facetsResultsContainer,
            params Expression<Func<TContent, object>>[] fieldSelectors)
            where TContent : IContent
        {
            if (!fieldSelectors.Any())
            {
                throw new ArgumentException("'fieldSelectors' needs to contain at least one expression.");
            }

            return GetFacetFor<StatisticalFacet>(
                facetsResultsContainer: facetsResultsContainer,
                facetName: string.Format(provider: CultureInfo.InvariantCulture, format: "StatisticalFacet_{0}", arg0: string.Join("_", fieldSelectors.Select(x => x.GetFieldPath()))));
        }
    }
}