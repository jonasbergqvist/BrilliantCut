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
        int SortOrder { get; set; }
        IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults);

        ISearch Filter(IContent content, ISearch query, IEnumerable<object> values);
        ISearch AddFasetToQuery(ISearch query);
    }
}
