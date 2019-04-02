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

    [ContentType]
    [AvailableContentTypes(availability: Availability.None)]
    public class FacetContent : BasicContent, IFacetContent
    {
        public virtual string ApplicationId { get; set; }

        public IEnumerable<string> CategoryNames { get; internal set; }

        public virtual string Code { get; set; }

        public virtual string DefaultCurrency { get; set; }

        public virtual string DefaultImageUrl { get; set; }

        public virtual double? DefaultPriceValue { get; set; }

        public IEnumerable<Inventory> Inventories { get; internal set; }

        public virtual string LengthBase { get; set; }

        public virtual string LinkUrl { get; set; }

        public virtual int? MetaClassId { get; set; }

        public IEnumerable<ContentReference> NodeLinks { get; internal set; }

        public IEnumerable<Price> Prices { get; internal set; }

        public IEnumerable<ContentReference> ProductLinks { get; internal set; }

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

        public virtual DateTime? StartPublish { get; set; }

        public virtual DateTime? StopPublish { get; set; }

        public virtual string ThumbnailPath { get; set; }

        public virtual string ThumbnailUrl { get; set; }

        public IEnumerable<ContentReference> VariationLinks { get; internal set; }

        public virtual string WeightBase { get; set; }
    }
}