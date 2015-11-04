using System;
using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Find;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;

namespace BrilliantCut.Core.Filters
{
    public interface IFilterContent
    {
        String Name { get; }
        String Description { get; }

        IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode, IContent currentContent);
        ISearch AddfacetToQuery(ISearch query, FacetFilterSetting setting);
        ISearch Filter(IContent content, ISearch query, IEnumerable<object> values);
    }
}
