using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Find;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.FilterSettings;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Filters
{
    public interface IFilterContent
    {
        String Name { get; }
        String Description { get; }

        IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode);
        ISearch AddfacetToQuery(ISearch query, FacetFilterSetting setting);
        ISearch Filter(IContent content, ISearch query, IEnumerable<object> values);
    }
}
