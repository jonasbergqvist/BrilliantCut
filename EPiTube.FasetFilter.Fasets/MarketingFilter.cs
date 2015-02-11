//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Beque.Find.Core;
//using EPiServer.Commerce.Catalog.ContentTypes;
//using EPiServer.Core;
//using EPiServer.Find;
//using EPiServer.Find.Framework;

//namespace Beque.ExampleSite.Find
//{
//    public class MarketingFilter : FilterContentBase<EntryContentBase, string>
//    {
//        public override string Name
//        {
//            get { return "Markets"; }
//        }

//        public override ITypeSearch<EntryContentBase> Filter(IContent currentCntent, ITypeSearch<EntryContentBase> query, IEnumerable<string> values)
//        {
//            throw new NotImplementedException();
//        }

//        public override IDictionary<string, string> GetFilterOptions(IContent currentContent)
//        {
//            var searchResults = SearchClient.Instance.Search<EntryContentBase>()
//                .TermsFacetFor(x => x.MarketFilter)
//                .Take(0)
//                .GetResult();

//            var authorCounts = searchResults
//                .TermsFacetFor(x => x.MarketFilter).Terms;

//            return authorCounts.ToDictionary(k => k.Term, v => v.Term);
//        }
//    }
//}