using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Find;
using BrilliantCut.Core;

namespace BrilliantCut.Core.Extensions
{
    public static class FilterConfigurationExtensions
    {
        public static FilterConfiguration DefaultPriceFilter(this FilterConfiguration filterConfiguration)
        {
            return filterConfiguration.RangeFacet<VariationContent, double>(x => x.DefaultPriceValue().Value,
                (builder, values) => builder
                    .And(x => x.DefaultPriceValue().Value.GreaterThan(values.Min() - 0.1))
                    .And(x => x.DefaultPriceValue().Value.LessThan(values.Max() + 0.1)));
        }

        public static FilterConfiguration InventoryFilter(this FilterConfiguration filterConfiguration)
        {
            return filterConfiguration.RangeFacet<VariationContent, double>(x => x.TotalInStock().Value,
                (builder, values) => builder
                    .And(x => x.TotalInStock().Value.GreaterThan(values.Min() - 0.1))
                    .And(x => x.TotalInStock().Value.LessThan(values.Max() + 0.1)));
        }
    }
}
