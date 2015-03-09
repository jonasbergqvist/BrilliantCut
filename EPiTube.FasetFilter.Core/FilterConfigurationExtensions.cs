﻿using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Find;

namespace EPiTube.FasetFilter.Core
{
    public static class FilterConfigurationExtensions
    {
        public static FilterConfiguration DefaultPriceFilter(this FilterConfiguration filterConfiguration)
        {
            return filterConfiguration.RangeFacet<VariationContent>(x => x.DefaultPrice(),
                (builder, values) => builder
                    .And(x => x.DefaultPrice().GreaterThan(values.Min() - 0.1))
                    .And(x => x.DefaultPrice().LessThan(values.Max() + 0.1)));
        }

        public static FilterConfiguration InventoryFilter(this FilterConfiguration filterConfiguration)
        {
            return filterConfiguration.RangeFacet<VariationContent>(x => x.TotalInStock(),
                (builder, values) => builder
                    .And(x => x.TotalInStock().GreaterThan(values.Min() - 0.1))
                    .And(x => x.TotalInStock().LessThan(values.Max() + 0.1)));
        }
    }
}
