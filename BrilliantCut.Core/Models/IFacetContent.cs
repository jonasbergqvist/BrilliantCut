// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFacetContent.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    using System;
    using System.Collections.Generic;

    using EPiServer.Commerce.SpecializedProperties;
    using EPiServer.Core;

    /// <summary>
    /// Interface IFacetContent
    /// Implements the <see cref="EPiServer.Core.IContent" />
    /// </summary>
    /// <seealso cref="EPiServer.Core.IContent" />
    public interface IFacetContent : IContent
    {
        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        string ApplicationId { get; }

        /// <summary>
        /// Gets the category names.
        /// </summary>
        /// <value>The category names.</value>
        IEnumerable<string> CategoryNames { get; }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        string Code { get; }

        /// <summary>
        /// Gets the default currency.
        /// </summary>
        /// <value>The default currency.</value>
        string DefaultCurrency { get; }

        /// <summary>
        /// Gets the default image URL.
        /// </summary>
        /// <value>The default image URL.</value>
        string DefaultImageUrl { get; }

        /// <summary>
        /// Gets the default price value.
        /// </summary>
        /// <value>The default price value.</value>
        double? DefaultPriceValue { get; }

        /// <summary>
        /// Gets the inventories.
        /// </summary>
        /// <value>The inventories.</value>
        IEnumerable<Inventory> Inventories { get; }

        /// <summary>
        /// Gets the length base.
        /// </summary>
        /// <value>The length base.</value>
        string LengthBase { get; }

        /// <summary>
        /// Gets the link URL.
        /// </summary>
        /// <value>The link URL.</value>
        string LinkUrl { get; }

        /// <summary>
        /// Gets the meta class identifier.
        /// </summary>
        /// <value>The meta class identifier.</value>
        int? MetaClassId { get; }

        /// <summary>
        /// Gets the node links.
        /// </summary>
        /// <value>The node links.</value>
        IEnumerable<ContentReference> NodeLinks { get; }

        /// <summary>
        /// Gets the prices.
        /// </summary>
        /// <value>The prices.</value>
        IEnumerable<Price> Prices { get; }

        /// <summary>
        /// Gets the product links.
        /// </summary>
        /// <value>The product links.</value>
        IEnumerable<ContentReference> ProductLinks { get; }

        /// <summary>
        /// Gets the property collection.
        /// </summary>
        /// <value>The property collection.</value>
        PropertyDataCollection PropertyCollection { get; }

        /// <summary>
        /// Gets the start publish.
        /// </summary>
        /// <value>The start publish.</value>
        DateTime? StartPublish { get; }

        /// <summary>
        /// Gets the stop publish.
        /// </summary>
        /// <value>The stop publish.</value>
        DateTime? StopPublish { get; }

        /// <summary>
        /// Gets the thumbnail path.
        /// </summary>
        /// <value>The thumbnail path.</value>
        string ThumbnailPath { get; }

        /// <summary>
        /// Gets the variation links.
        /// </summary>
        /// <value>The variation links.</value>
        IEnumerable<ContentReference> VariationLinks { get; }

        /// <summary>
        /// Gets the weight base.
        /// </summary>
        /// <value>The weight base.</value>
        string WeightBase { get; }
    }
}