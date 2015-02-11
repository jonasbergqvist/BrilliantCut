using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Find;

namespace EPiTube.FasetFilter.Core
{
    public interface IFilterContent
    {
        String Name { get; }
        String Description { get; }
        IDictionary<string, object> GetFilterOptions(ContentReference contentLink);
    }

    public interface IFilterContent<out TContentData> : IFilterContent
        where TContentData : IContent
    {
        ITypeSearch<TContentData> Filter(IContent content, ISearch query, IEnumerable<object> values);
    }
}
