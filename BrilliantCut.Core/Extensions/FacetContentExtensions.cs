// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetContentExtensions.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.SpecializedProperties;
    using EPiServer.Core;

    /// <summary>
    /// Class FacetContentExtensions.
    /// </summary>
    public static class FacetContentExtensions
    {
        /// <summary>
        /// Gets the inventories for the specified facet content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Inventory"/> for the <see cref="IFacetContent"/>.</returns>
        /// <exception cref="NotSupportedException">This is only for projection</exception>
        public static IEnumerable<Inventory> Inventories(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        /// <summary>
        /// Gets the node links for the specified facet content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentReference"/> for the <see cref="IFacetContent"/>.</returns>
        /// <exception cref="NotSupportedException">This is only for projection</exception>
        public static IEnumerable<ContentReference> NodeLinks(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        /// <summary>
        /// Gets the prices for the specified facet content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Price"/> for the <see cref="IFacetContent"/>.</returns>
        /// <exception cref="NotSupportedException">This is only for projection</exception>
        public static IEnumerable<Price> Prices(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        /// <summary>
        /// Gets the product links for the specified facet content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentReference"/> for the <see cref="IFacetContent"/>.</returns>
        /// <exception cref="NotSupportedException">This is only for projection</exception>
        public static IEnumerable<ContentReference> ProductLinks(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        /// <summary>
        /// Gets the variation links for the specified facet content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentReference"/> for the <see cref="IFacetContent"/>.</returns>
        /// <exception cref="NotSupportedException">This is only for projection</exception>
        public static IEnumerable<ContentReference> VariationLinks(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }
    }
}