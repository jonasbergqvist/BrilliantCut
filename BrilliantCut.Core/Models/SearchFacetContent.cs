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

    public class SearchFacetContent
    {
        public IEnumerable<string> CategoryNames { get; set; }

        public string Code { get; set; }

        public Guid ContentGuid { get; set; }

        public ContentReference ContentLink { get; set; }

        public int ContentTypeID { get; set; }

        public string DefaultCurrency { get; set; }

        public string DefaultImageUrl { get; set; }

        public double? DefaultPriceValue { get; set; }

        public IEnumerable<Inventory> Inventories { get; set; }

        public bool IsDeleted { get; set; }

        public string LengthBase { get; set; }

        public string LinkUrl { get; set; }

        public int? MetaClassId { get; set; }

        public string Name { get; set; }

        public IEnumerable<ContentReference> NodeLinks { get; set; }

        public ContentReference ParentLink { get; set; }

        public IEnumerable<Price> Prices { get; set; }

        public IEnumerable<ContentReference> ProductLinks { get; set; }

        public PropertyDataCollection Property { get; set; }

        public PropertyDataCollection PropertyCollection { get; set; }

        public DateTime? StartPublish { get; set; }

        public DateTime? StopPublish { get; set; }

        public string ThumbnailPath { get; set; }

        public IEnumerable<ContentReference> VariationLinks { get; set; }

        public string WeightBase { get; set; }
    }
}