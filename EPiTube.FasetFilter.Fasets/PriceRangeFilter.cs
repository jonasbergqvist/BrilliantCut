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

        public override IDictionary<string, double> GetFilterOptions(IContent currentContent)
        {
            var searchResults = SearchClient.Instance.Search<CatalogContentBase>()
                .StatisticalFacetFor(x => x.DefaultPrice())
                .Take(0)
                .GetResult();

            var authorCounts = searchResults
                .StatisticalFacetFor(x => x.DefaultPrice());

            return new Dictionary<string, double>() { { "pricemin", authorCounts.Min }, { "pricemax", authorCounts.Max } };
        }
    }
}