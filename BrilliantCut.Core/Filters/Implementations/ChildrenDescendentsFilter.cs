// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildrenDescendentsFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Cms;

    [RadiobuttonFilter]
    public class ChildrenDescendentsFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get
            {
                return "Deep";
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
            if (currentCntent is ProductContent)
            {
                return query
                    .Filter(
                        x => x.ParentProducts().MatchContained(y => y.ID, currentCntent.ContentLink.ID))
                    .Filter(x => !x.ContentLink.Match(currentCntent.ContentLink));
            }

            string[] valueArray = values as string[] ?? values.ToArray();
            if (valueArray.Any() && valueArray.First() == "Children")
            {
                return query.Filter(x => x.ParentLink.Match(currentCntent.ContentLink.ToReferenceWithoutVersion()));
            }

            return query.Filter(
                x => x.Ancestors().Match(currentCntent.ContentLink.ToReferenceWithoutVersion().ToString()));
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            if (mode != ListingMode.WidgetListing)
            {
                yield return new FilterOptionModel("Children", "Children", "Children", true, -1);
            }

            yield return new FilterOptionModel("Descendents", "Descendents", "Descendents", false, -1);
        }
    }
}