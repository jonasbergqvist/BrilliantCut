using System;
using System.Collections.Generic;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiTube.FasetFilter.Core
{
    public interface IEPiFasetModel
    {
        string Name { get; }
        Guid ContentGuid { get; }
        ContentReference ContentLink { get; }
        bool IsDeleted { get; }
        ContentReference ParentLink { get; }

        DateTime? StartPublish { get; }
        DateTime? StopPublish { get; }
        int ContentTypeID { get; }
        string ApplicationId { get; }
        int MetaClassId { get; }
        string DefaultCurrency { get; }
        string WeightBase { get; }
        string LengthBase { get; }
        string Code { get; }
        string ThumbnailPath { get; }
        double DefaultPrice { get; }

        IEnumerable<ContentReference> NodeLinks { get; }
        IEnumerable<ContentReference> ProductLinks { get; }
        IEnumerable<ContentReference> VariationLinks { get; }
        IEnumerable<Price> Prices { get; }
        IEnumerable<Inventory> Inventories { get; }
        PropertyDataCollection PropertyCollection { get; }
    }

    [ContentType]
    public class EPiTubeModel : BasicContent, IEPiFasetModel
    {
        public virtual DateTime? StartPublish { get; set; }
        public virtual DateTime? StopPublish { get; set; }
        public virtual string ApplicationId { get; set; }
        public virtual int MetaClassId  { get; set; }
        public virtual string DefaultCurrency  { get; set; }
        public virtual string WeightBase  { get; set; }
        public virtual string LengthBase { get; set; }
        public virtual string Code { get; set; }
        public virtual string ThumbnailPath { get; set; }
        public virtual double DefaultPrice { get; set; }

        public IEnumerable<ContentReference> NodeLinks { get; internal set; }
        public IEnumerable<ContentReference> ProductLinks { get; internal set; }
        public IEnumerable<ContentReference> VariationLinks { get; internal set; }
        public IEnumerable<Price> Prices { get; internal set; }
        public IEnumerable<Inventory> Inventories { get; internal set; }
        public PropertyDataCollection PropertyCollection { get { return Property; } internal set { Property = value; } }
    }
}
