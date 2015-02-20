using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiTube.FasetFilter.Core;

namespace EPiTube.FasetFilter.Fasets
{
    public class MarketingFilter : FilterContentBase<EntryContentBase, string>
    {
        public override string Name
        {
            get { return "Markets"; }
        }

        public override ITypeSearch<EntryContentBase> Filter(IContent currentCntent, ITypeSearch<EntryContentBase> query, IEnumerable<string> values)
        {
            var marketFilter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            marketFilter = values.Aggregate(marketFilter, (current, value) => current.Or(x => x.MarketFilter.Match(value)));

            return query.Filter(marketFilter);
        }

        public override IDictionary<string, string> GetFilterOptionsFromResult(SearchResults<EPiTubeModel> searchResults)
        {
            var authorCounts = searchResults
                .TermsFacetFor<EntryContentBase>(x => x.MarketFilter).Terms;

            return authorCounts.ToDictionary(k => k.Term, v => v.Term);
        }

        public override ITypeSearch<EntryContentBase> AddFasetToQuery(ITypeSearch<EntryContentBase> query)
        {
            return query.TermsFacetFor(x => x.MarketFilter);
        }
    }
}