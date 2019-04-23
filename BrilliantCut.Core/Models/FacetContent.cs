// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetContent.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    using System;
    using System.Collections.Generic;

    using EPiServer.Commerce.SpecializedProperties;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;

    /// <summary>
    /// Class FacetContent.
    /// Implements the <see cref="EPiServer.Core.BasicContent" />
    /// Implements the <see cref="BrilliantCut.Core.Models.IFacetContent" />
    /// </summary>
    /// <seealso cref="EPiServer.Core.BasicContent" />
    /// <seealso cref="BrilliantCut.Core.Models.IFacetContent" />
    [ContentType]
    [AvailableContentTypes(availability: Availability.None)]
    public class FacetContent : BasicContent, IFacetContent
    {
        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public virtual string ApplicationId { get; set; }

        /// <summary>
        /// Gets the category names.
        /// </summary>
        /// <value>The category names.</value>
        public IEnumerable<string> CategoryNames { get; internal set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public virtual string Code { get; set; }

        /// <summary>
        /// Gets or sets the default currency.
        /// </summary>
        /// <value>The default currency.</value>
        public virtual string DefaultCurrency { get; set; }

        /// <summary>
        /// Gets or sets the default image URL.
        /// </summary>
        /// <value>The default image URL.</value>
        public virtual string DefaultImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the default price value.
        /// </summary>
        /// <value>The default price value.</value>
        public virtual double? DefaultPriceValue { get; set; }

        /// <summary>
        /// Gets the inventories.
        /// </summary>
        /// <value>The inventories.</value>
        public IEnumerable<Inventory> Inventories { get; internal set; }

        /// <summary>
        /// Gets or sets the length base.
        /// </summary>
        /// <value>The length base.</value>
        public virtual string LengthBase { get; set; }

        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        /// <value>The link URL.</value>
        public virtual string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the meta class identifier.
        /// </summary>
        /// <value>The meta class identifier.</value>
        public virtual int? MetaClassId { get; set; }

        /// <summary>
        /// Gets the node links.
        /// </summary>
        /// <value>The node links.</value>
        public IEnumerable<ContentReference> NodeLinks { get; internal set; }

        /// <summary>
        /// Gets the prices.
        /// </summary>
        /// <value>The prices.</value>
        public IEnumerable<Price> Prices { get; internal set; }

        /// <summary>
        /// Gets the product links.
        /// </summary>
        /// <value>The product links.</value>
        public IEnumerable<ContentReference> ProductLinks { get; internal set; }

        /// <summary>
        /// Gets the property collection.
        /// </summary>
        /// <value>The property collection.</value>
        public PropertyDataCollection PropertyCollection
        {
            get
            {
                return this.Property;
            }

            internal set
            {
                this.Property = value;
            }
        }

        /// <summary>
        /// Gets or sets the start publish date.
        /// </summary>
        /// <value>The start publish date.</value>
        public virtual DateTime? StartPublish { get; set; }

        /// <summary>
        /// Gets or sets the stop publish date.
        /// </summary>
        /// <value>The stop publish date.</value>
        public virtual DateTime? StopPublish { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail path.
        /// </summary>
        /// <value>The thumbnail path.</value>
        public virtual string ThumbnailPath { get; set; }

        /// <summary>
        /// Gets or sets the thumbnail URL.
        /// </summary>
        /// <value>The thumbnail URL.</value>
        public virtual string ThumbnailUrl { get; set; }

        /// <summary>
        /// Gets the variation links.
        /// </summary>
        /// <value>The variation links.</value>
        public IEnumerable<ContentReference> VariationLinks { get; internal set; }

        /// <summary>
        /// Gets or sets the weight base.
        /// </summary>
        /// <value>The weight base.</value>
        public virtual string WeightBase { get; set; }
    }
}