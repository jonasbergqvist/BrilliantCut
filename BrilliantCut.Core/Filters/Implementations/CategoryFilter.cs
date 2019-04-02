// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Api.Facets;
    using EPiServer.Find.Framework;

    public class CategoryFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get
            {
                return "Category";
            }
        }

        public override ITypeSearch<CatalogContentBase> AddFacetToQuery(
            ITypeSearch<CatalogContentBase> query,
            FacetFilterSetting setting)
        {
            return query.TermsFacetFor(
                x => x.CategoryNames(),
                request =>
                    {
                        if (setting.MaxFacetHits.HasValue)
                        {
                            request.Size = setting.MaxFacetHits;
                        }
                    });
        }

        public override ITypeSearch<CatalogContentBase> Filter(
            IContent currentCntent,
            ITypeSearch<CatalogContentBase> query,
            IEnumerable<string> values)
        {
            FilterBuilder<EntryContentBase> filter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            filter = values.Aggregate(
                seed: filter,
                func: (current, value) => current.Or(x => x.CategoryNames().MatchCaseInsensitive(value)));

            return query.Filter(filter: filter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            IEnumerable<TermCount> facet = searchResults.TermsFacetFor<CatalogContentBase>(x => x.CategoryNames()).Terms;

            return facet.Select(
                authorCount => new FilterOptionModel(
                    "category" + authorCount.Term,
                    string.Format(
                        provider: CultureInfo.InvariantCulture,
                        format: "{0} ({1})",
                        arg0: authorCount.Term,
                        arg1: authorCount.Count),
                    value: authorCount.Term,
                    defaultValue: false,
                    count: authorCount.Count));
        }
    }
}