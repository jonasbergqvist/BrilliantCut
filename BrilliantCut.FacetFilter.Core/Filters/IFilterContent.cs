using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Find;
using BrilliantCut.FacetFilter.Core.Extensions;
using BrilliantCut.FacetFilter.Core.FilterSettings;
using BrilliantCut.FacetFilter.Core.Models;

namespace BrilliantCut.FacetFilter.Core.Filters
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
