using System;
using EPiServer.Commerce.Catalog.ContentTypes;

namespace EPiTube.FasetFilter.Core
{
    public class FilterContentModelType
    {
        public IFilterContent Filter { get; set; }
        public Type ContentType { get; set; }
        public Type QueryContentType { get; set; }
        public bool FasetAdded { get; set; }
        public bool HasGenericArgument { get; set; }
    }
}
