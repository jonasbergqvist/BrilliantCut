//using System;
//using System.Collections.Generic;
//using System.Linq;
//using EPiServer.Commerce.Catalog.ContentTypes;
//using EPiServer.Core;
//using EPiServer.Find;
//using EPiServer.ServiceLocation;
//using EPiTube.FasetFilter.Core;
//using EPiTube.FasetFilter.Core.DataAnnotation;

//namespace EPiTube.FasetFilter.Fasets
//{
//    [ServiceConfiguration, TextboxFilter]
//    public class ColdeFilter : FilterContentBase<CatalogContentBase, string>
//    {
//        public override string Name
//        {
//            get { return "CodeFilter"; }
//        }

//        public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<string> values)
//        {
//            var valueArray = values as string[] ?? values.ToArray();
//            if (!valueArray.Any())
//            {
//                return query;
//            }

//            var value = valueArray.First();
//            if (String.IsNullOrEmpty(value))
//            {
//                return query;
//            }

//            return query.For(value)
//                .Include(x => x.Code().AnyWordBeginsWith(value));
//        }

//        public override IDictionary<string, string> GetFilterOptionsFromResult(SearchResults<EPiTubeModel> searchResults)
//        {
//            return new Dictionary<string, string>() { { "CodeFilter", string.Empty } };
//        }

//        public override ITypeSearch<CatalogContentBase> AddFasetToQuery(ITypeSearch<CatalogContentBase> query)
//        {
//            return query;
//        }
//    }
//}