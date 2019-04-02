// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActiveFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Cms;

    [RadiobuttonFilter]
    public class IsActiveFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get
            {
                return "Active";
            }
        }

        public override ITypeSearch<CatalogContentBase> AddFacetToQuery(
            ITypeSearch<CatalogContentBase> query,
            FacetFilterSetting setting)
        {
            return query;
        }

        public override ITypeSearch<CatalogContentBase> Filter(
            IContent currentCntent,
            ITypeSearch<CatalogContentBase> query,
            IEnumerable<string> values)
        {
            string value = values.FirstOrDefault();
            if (value == null || value == "all")
            {
                return query;
            }

            return query.CurrentlyPublished().ExcludeDeleted();
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            yield return new FilterOptionModel("all", "All", false, false, -1);
            yield return new FilterOptionModel("active", "Active", false, false, -1);

            // yield return new FilterOptionModel("unactive", "Unactive", false, false, -1);
        }
    }
}