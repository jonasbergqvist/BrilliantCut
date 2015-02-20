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
        IDictionary<string, object> GetFilterOptions(SearchResults<EPiTubeModel> searchResults);
        //IDictionary<string, object> GetFilterOptions();
    }

    public interface IFilterContent<out TContentData> : IFilterContent
    {
        ITypeSearch<TContentData> Filter(IContent content, ISearch query, IEnumerable<object> values);
        ITypeSearch<TContentData> AddFasetToQuery(ISearch query);
    }
}
