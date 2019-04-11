// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchFacetContent.cs" company="Jonas Bergqvist">
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
    /// Class SearchFacetContent.
    /// </summary>
    public class SearchFacetContent
    {
        /// <summary>
        /// Gets or sets the category names.
        /// </summary>
        /// <value>The category names.</value>
        public IEnumerable<string> CategoryNames { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the content unique identifier.
        /// </summary>
        /// <value>The content unique identifier.</value>
        public Guid ContentGuid { get; set; }

        /// <summary>
        /// Gets or sets the content link.
        /// </summary>
        /// <value>The content link.</value>
        public ContentReference ContentLink { get; set; }

        /// <summary>
        /// Gets or sets the content type identifier.
        /// </summary>
        /// <value>The content type identifier.</value>
        public int ContentTypeID { get; set; }

        /// <summary>
        /// Gets or sets the default currency.
        /// </summary>
        /// <value>The default currency.</value>
        public string DefaultCurrency { get; set; }

        /// <summary>
        /// Gets or sets the default image URL.
        /// </summary>
        /// <value>The default image URL.</value>
        public string DefaultImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the default price value.
        /// </summary>
        /// <value>The default price value.</value>
        public double? DefaultPriceValue { get; set; }

        /// <summary>
        /// Gets or sets the inventories.
        /// </summary>
        /// <value>The inventories.</value>
        public IEnumerable<Inventory> Inventories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value><c>true</c> if this instance is deleted; otherwise, <c>false</c>.</value>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the length base.
        /// </summary>
        /// <value>The length base.</value>
        public string LengthBase { get; set; }

        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        /// <value>The link URL.</value>
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the meta class identifier.
        /// </summary>
        /// <value>The meta class identifier.</value>
        public int? MetaClassId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the node links.
        /// </summary>
        /// <value>The node links.</value>
        public IEnumerable<ContentReference> NodeLinks { get; set; }

        /// <summary>
        /// Gets or sets the parent link.
        /// </summary>
        /// <value>The parent link.</value>
        public ContentReference ParentLink { get; set; }

        /// <summary>
        /// Gets or sets the prices.
        /// </summary>
        /// <value>The prices.</value>
        public IEnumerable<Price> Prices { get; set; }

        /// <summary>
        /// Gets or sets the product links.
        /// </summary>
        /// <value>The product links.</value>
        public IEnumerable<ContentReference> ProductLinks { get; set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyDataCollection Property { get; set; }

        /// <summary>
        /// Gets or sets the property collection.
        /// </summary>
        /// <value>The property collection.</value>
        public PropertyDataCollection PropertyCollection { get; set; }

        /// <summary>
        /// Gets or sets the start publish.
        /// </summary>
        /// <value>The start publish.</value>
        public DateTime? StartPublish { get; set; }

        /// <summary>
        /// Gets or sets the stop publish.
        /// </summary>
        /// <value>The stop publish.</value>
        public DateTime? StopPublish { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail path.
        /// </summary>
        /// <value>The thumbnail path.</value>
        public string ThumbnailPath { get; set; }

        /// <summary>
        /// Gets or sets the variation links.
        /// </summary>
        /// <value>The variation links.</value>
        public IEnumerable<ContentReference> VariationLinks { get; set; }

        /// <summary>
        /// Gets or sets the weight base.
        /// </summary>
        /// <value>The weight base.</value>
        public string WeightBase { get; set; }
    }
}