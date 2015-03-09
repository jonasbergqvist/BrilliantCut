using System;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiTube.FasetFilter.Core.Settings;

namespace EPiTube.FasetFilter.Core
{
    public class FilterContentModelType
    {
        public IFilterContent Filter { get; set; }
        public Type ContentType { get; set; }
        public Type QueryContentType { get; set; }
        public bool FasetAdded { get; set; }
        public bool HasGenericArgument { get; set; }
        public FasetFilterSetting Setting { get; set; }
    }
}
