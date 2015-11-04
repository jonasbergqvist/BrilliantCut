using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;

namespace BrilliantCut.Core.Filters.Implementations
{
    public class MarketsFilter : FilterContentBase<EntryContentBase, string>
    {
        public override string Name
        {
            get { return "Markets"; }
        }

        public override ITypeSearch<EntryContentBase> Filter(IContent currentCntent, ITypeSearch<EntryContentBase> query, IEnumerable<string> values)
        {
            var marketFilter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            marketFilter = values.Aggregate(marketFilter, (current, value) => current.Or(x => x.SelectedMarkets().MatchCaseInsensitive(value)));

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode, IContent currentContent)
        {
            var facet = searchResults
                .TermsFacetFor<EntryContentBase>(x => x.SelectedMarkets()).Terms;

            return facet.Select(authorCount => new FilterOptionModel("marketing" + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count));
        }

        public override ITypeSearch<EntryContentBase> AddfacetToQuery(ITypeSearch<EntryContentBase> query, FacetFilterSetting setting)
        {
            return query.TermsFacetFor(x => x.SelectedMarkets());
        }
    }
}