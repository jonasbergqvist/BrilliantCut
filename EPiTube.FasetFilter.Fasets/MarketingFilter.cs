using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiServer.ServiceLocation;
using EPiTube.FasetFilter.Core;
using EPiTube.FasetFilter.Core.DataAnnotation;

namespace EPiTube.FasetFilter.Fasets
{
    [ServiceConfiguration]
    public class MarketingFilter : FilterContentBase<EntryContentBase, string>
    {
        public override string Name
        {
            get { return "Markets"; }
        }

        public override ITypeSearch<EntryContentBase> Filter(IContent currentCntent, ITypeSearch<EntryContentBase> query, IEnumerable<string> values)
        {
            var marketFilter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            marketFilter = values.Aggregate(marketFilter, (current, value) => current.Or(x => x.SelectedMarkets().Match(value)));

            return query.Filter(marketFilter);
        }

        public override IDictionary<string, string> GetFilterOptionsFromResult(SearchResults<EPiTubeModel> searchResults)
        {
            var authorCounts = searchResults
                .TermsFacetFor<EntryContentBase>(x => x.SelectedMarkets()).Terms;

            return authorCounts.ToDictionary(k => String.Format(CultureInfo.InvariantCulture, "{0} ({1})", k.Term, k.Count), v => v.Term);
        }

        public override ITypeSearch<EntryContentBase> AddFasetToQuery(ITypeSearch<EntryContentBase> query)
        {
            return query.TermsFacetFor(x => x.SelectedMarkets());
        }
    }
}