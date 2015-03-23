//using System;
//using System.Collections.Generic;
//using System.Linq;
//using EPiServer.Commerce.Catalog.ContentTypes;
//using EPiServer.Core;
//using EPiServer.Find;

//namespace EPiTube.facetFilter.Core.Filters
//{
//    public class DateTimeFaset : facetBase<CatalogContentBase, DateTime>
//    {
//        public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<DateTime> values)
//        {
//            var value = values.FirstOrDefault();
//            if (value == DateTime.MinValue)
//            {
//                return query;
//            }


//        }

//        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
//        {
//            throw new NotImplementedException();
//        }

//        public override ITypeSearch<CatalogContentBase> AddfacetToQuery(ITypeSearch<CatalogContentBase> query)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
