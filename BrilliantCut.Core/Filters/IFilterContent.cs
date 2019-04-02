// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterContent.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System.Collections.Generic;

    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Core;
    using EPiServer.Find;

    public interface IFilterContent
    {
        string Description { get; }

        string Name { get; }

        ISearch AddFacetToQuery(ISearch query, FacetFilterSetting setting);

        ISearch Filter(IContent content, ISearch query, IEnumerable<object> values);

        IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent);
    }
}