// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MarketsFilter.cs" company="Jonas Bergqvist">
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

    public class MarketsFilter : FilterContentBase<EntryContentBase, string>
    {
        public override string Name
        {
            get
            {
                return "Markets";
            }
        }

        public override ITypeSearch<EntryContentBase> AddFacetToQuery(
            ITypeSearch<EntryContentBase> query,
            FacetFilterSetting setting)
        {
            return query.TermsFacetFor(x => x.SelectedMarkets());
        }

        public override ITypeSearch<EntryContentBase> Filter(
            IContent currentCntent,
            ITypeSearch<EntryContentBase> query,
            IEnumerable<string> values)
        {
            FilterBuilder<EntryContentBase> marketFilter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            marketFilter = values.Aggregate(
                seed: marketFilter,
                func: (current, value) =>
                    current.Or(x => x.SelectedMarkets().MatchCaseInsensitive(value)));

            return query.Filter(filter: marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            IEnumerable<TermCount> facet = searchResults.TermsFacetFor<EntryContentBase>(x => x.SelectedMarkets())
                .Terms;

            return facet.Select(
                authorCount => new FilterOptionModel(
                    "marketing" + authorCount.Term,
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