using System;
using EPiServer.Commerce.Catalog.ContentTypes;

namespace EPiTube.FasetFilter.Core
{
    public class FilterContentModelType
    {
        public IFilterContent<object> Filter { get; set; }
        public Type ContentType { get; set; }
    }
}
