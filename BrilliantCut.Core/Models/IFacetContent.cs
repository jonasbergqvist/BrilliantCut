using System;
using System.Collections.Generic;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;

namespace BrilliantCut.Core.Models
{
    public interface IFacetContent : IContent
    {
        DateTime? StartPublish { get; }
        DateTime? StopPublish { get; }
        int ContentTypeID { get; }
        string ApplicationId { get; }
        int? MetaClassId { get; }
        string DefaultCurrency { get; }
        string WeightBase { get; }
        string LengthBase { get; }
        string Code { get; }
        string LinkUrl { get; }
        string ThumbnailPath { get; }
        string DefaultImageUrl { get; }
        double? DefaultPriceValue { get; }

        IEnumerable<string> CategoryNames { get; }
        IEnumerable<ContentReference> NodeLinks { get; }
        IEnumerable<ContentReference> ProductLinks { get; }
        IEnumerable<ContentReference> VariationLinks { get; }
        IEnumerable<Price> Prices { get; }
        IEnumerable<Inventory> Inventories { get; }
        PropertyDataCollection PropertyCollection { get; }
    }
}
