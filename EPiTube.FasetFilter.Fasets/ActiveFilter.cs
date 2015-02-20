//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using EPiServer.Commerce.Catalog.ContentTypes;
//using EPiServer.Core;
//using EPiServer.Find;
//using EPiTube.FasetFilter.Core;

//namespace EPiTube.FasetFilter.Fasets
//{
//    public class IsActiveFilter : FilterContentBase<CatalogContentBase, bool>
//    {
//        public override string Name
//        {
//            get { throw new NotImplementedException(); }
//        }

//        public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<bool> values)
//        {
//            query.Filter(x => x.)
//        }

//        public override IDictionary<string, bool> GetFilterOptionsFromResult(EPiServer.Find.SearchResults<EPiTubeModel> searchResults)
//        {
//            return new Dictionary<string, bool>({{"Active", true}});
//        }

//        public override EPiServer.Find.ITypeSearch<CatalogContentBase> AddFasetToQuery(EPiServer.Find.ITypeSearch<CatalogContentBase> query)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
