using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Find;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Filters
{
    public interface IFilterContent
    {
        String Name { get; }
        String Description { get; }

        IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults);
        ISearch AddfacetToQuery(ISearch query);
        ISearch Filter(IContent content, ISearch query, IEnumerable<object> values);
    }
}
