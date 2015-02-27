using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using EPiTube.FasetFilter.Core;
using EPiTube.FasetFilter.Core.DataAnnotation;

namespace EPiTube.FasetFilter.Fasets
{
    [ServiceConfiguration, SliderFilter]
    public class InventoryRangeFilter : FilterContentBase<VariationContent, double>
    {
        public override string Name
        {
            get { return "Inventory"; }
        }

        public override ITypeSearch<VariationContent> Filter(IContent currentCntent, ITypeSearch<VariationContent> query, IEnumerable<double> values)
        {
            var selectedValueArray = values.ToArray();
            if (!selectedValueArray.Any())
            {
                return query;
            }

            var min = selectedValueArray.Min();
            query = query.Filter(x => x.TotalInStock().GreaterThan(min - 0.1));

            var max = selectedValueArray.Max();
            query = query.Filter(x => x.TotalInStock().LessThan(max + 0.1));

            return query;
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        {
            var inStockFilter = searchResults
                .StatisticalFacetFor<VariationContent>(x => x.TotalInStock());

            yield return new FilterOptionModel("inventorymin", "min", inStockFilter.Min, 0);
            yield return new FilterOptionModel("inventorymax", "max", inStockFilter.Max, 100);
        }

        public override ITypeSearch<VariationContent> AddFasetToQuery(ITypeSearch<VariationContent> query)
        {
            return query.StatisticalFacetFor(x => x.TotalInStock());
        }
    }
}