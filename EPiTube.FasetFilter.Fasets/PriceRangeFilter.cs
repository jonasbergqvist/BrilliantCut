using System.Collections.Generic;
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
    [ServiceConfiguration, SliderFilter]
    public class PriceRangeFilter : FilterContentBase<VariationContent, double>
    {
        public override string Name
        {
            get { return "Price"; }
        }

        public override ITypeSearch<VariationContent> Filter(IContent currentCntent, ITypeSearch<VariationContent> query, IEnumerable<double> values)
        {
            //query = query.StatisticalFacetFor(x => x.DefaultPrice());

            var selectedValueArray = values.ToArray();
            if (!selectedValueArray.Any())
            {
                return query;
            }

            var min = selectedValueArray.Min();
            query = query.Filter(x => x.DefaultPrice().GreaterThan(min - 0.1));

            var max = selectedValueArray.Max();
            query = query.Filter(x => x.DefaultPrice().LessThan(max + 0.1));

            return query;
        }

        public override IDictionary<string, double> GetFilterOptionsFromResult(SearchResults<EPiTubeModel> searchResults)
        {
            var authorCounts = searchResults
                .StatisticalFacetFor<VariationContent>(x => x.DefaultPrice());

            return new Dictionary<string, double>() { { "pricemin", authorCounts.Min }, { "pricemax", authorCounts.Max } }; 
        }

        public override ITypeSearch<VariationContent> AddFasetToQuery(ITypeSearch<VariationContent> query)
        {
            return query.StatisticalFacetFor(x => x.DefaultPrice());
        }

        //public override IDictionary<string, double> GetDefaultFilterOptions()
        //{
        //    var searchResults = SearchClient.Instance.Search<VariationContent>()
        //        .StatisticalFacetFor(x => x.DefaultPrice())
        //        .Take(0)
        //        .GetResult();

        //    var authorCounts = searchResults
        //        .StatisticalFacetFor(x => x.DefaultPrice());

        //    return new Dictionary<string, double>() { { "pricemin", authorCounts.Min }, { "pricemax", authorCounts.Max } };
        //}

        //public override ITypeSearch<VariationContent> AddFasetToQuery(ITypeSearch<VariationContent> query)
        //{
        //    return query.StatisticalFacetFor(x => x.DefaultPrice());
        //}
    }
}