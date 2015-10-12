using System;
using System.Collections.Generic;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace BrilliantCut.Core.Models
{
    [ContentType, AvailableContentTypes(Availability.None)]
    public class FacetContent : BasicContent, IFacetContent
    {
        public virtual DateTime? StartPublish { get; set; }
        public virtual DateTime? StopPublish { get; set; }
        public virtual string ApplicationId { get; set; }
        public virtual int? MetaClassId  { get; set; }
        public virtual string DefaultCurrency  { get; set; }
        public virtual string WeightBase  { get; set; }
        public virtual string LengthBase { get; set; }
        public virtual string Code { get; set; }
        public virtual string ThumbnailPath { get; set; }
        public virtual double? DefaultPrice { get; set; }
        public virtual string ThumbnailUrl { get; set; }

        public IEnumerable<string> CategoryNames { get; internal set; }
        public IEnumerable<ContentReference> NodeLinks { get; internal set; }
        public IEnumerable<ContentReference> ProductLinks { get; internal set; }
        public IEnumerable<ContentReference> VariationLinks { get; internal set; }
        public IEnumerable<Price> Prices { get; internal set; }
        public IEnumerable<Inventory> Inventories { get; internal set; }
        public PropertyDataCollection PropertyCollection { get { return Property; } internal set { Property = value; } }
    }
}
