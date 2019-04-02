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

    public interface IFacetContent : IContent
    {
        string ApplicationId { get; }

        IEnumerable<string> CategoryNames { get; }

        string Code { get; }

        int ContentTypeID { get; }

        string DefaultCurrency { get; }

        string DefaultImageUrl { get; }

        double? DefaultPriceValue { get; }

        IEnumerable<Inventory> Inventories { get; }

        string LengthBase { get; }

        string LinkUrl { get; }

        int? MetaClassId { get; }

        IEnumerable<ContentReference> NodeLinks { get; }

        IEnumerable<Price> Prices { get; }

        IEnumerable<ContentReference> ProductLinks { get; }

        PropertyDataCollection PropertyCollection { get; }

        DateTime? StartPublish { get; }

        DateTime? StopPublish { get; }

        string ThumbnailPath { get; }

        IEnumerable<ContentReference> VariationLinks { get; }

        string WeightBase { get; }
    }
}