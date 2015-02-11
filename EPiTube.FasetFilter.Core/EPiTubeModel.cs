using System;
using System.Collections.Generic;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace EPiTube.FasetFilter.Core
{
    [ContentType]
    public class EPiTubeModel : BasicContent
    {
        //public virtual string Name { get; set; }
        //public virtual Guid ContentGuid { get; set; }
        //public virtual ContentReference ContentLink { get; set; }
        //public virtual bool IsDeleted { get; set; }
        //public virtual ContentReference ParentLink { get; set; }
        public virtual DateTime? StartPublish { get; set; }
        public virtual DateTime? StopPublish { get; set; }
        public virtual int ContentTypeId { get; set; }
        public virtual string ApplicationId { get; set; }
        public virtual int MetaClassId  { get; set; }
        public virtual string DefaultCurrency  { get; set; }
        public virtual string WeightBase  { get; set; }
        public virtual string LengthBase { get; set; }
        public virtual string Code { get; set; }
        public virtual bool HasChildren { get; set; }
        public virtual string ThumbnailPath { get; set; }
        public virtual double DefaultPrice { get; set; }

        public void Initialize(
            IEnumerable<ContentReference> nodeLinks,
            IEnumerable<ContentReference> productLinks,
            IEnumerable<ContentReference> variationLinks,
            IEnumerable<Price> prices,
            IEnumerable<Inventory> inventories)
        {
            NodeLinks = nodeLinks;
            ProductLinks = productLinks;
            VariationLinks = variationLinks;
            Prices = prices;
            Inventories = inventories;
        }

        public IEnumerable<ContentReference> NodeLinks { get; private set; }

        public IEnumerable<ContentReference> ProductLinks { get; private set; }

        public IEnumerable<ContentReference> VariationLinks { get; private set; }

        public IEnumerable<Price> Prices { get; private set; }

        public IEnumerable<Inventory> Inventories { get; private set; } 
    }
}
