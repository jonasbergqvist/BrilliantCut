// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterConfigurationExtensions.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Extensions
{
    using System.Linq;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Find;

    /// <summary>
    /// Class FilterConfigurationExtensions.
    /// </summary>
    public static class FilterConfigurationExtensions
    {
        /// <summary>
        /// Gets the default price filter.
        /// </summary>
        /// <param name="filterConfiguration">The filter configuration.</param>
        /// <returns>The <see cref="FilterConfiguration"/>.</returns>
        public static FilterConfiguration DefaultPriceFilter(this FilterConfiguration filterConfiguration)
        {
            if (filterConfiguration == null)
            {
                return null;
            }

            return filterConfiguration.RangeFacet<VariationContent, double>(
                x => x.DefaultPriceValue().Value,
                (builder, values) => builder.And(x => x.DefaultPriceValue().Value.GreaterThan(values.Min() - 0.1))
                    .And(x => x.DefaultPriceValue().Value.LessThan(values.Max() + 0.1)));
        }

        /// <summary>
        /// Gets the inventory filter.
        /// </summary>
        /// <param name="filterConfiguration">The filter configuration.</param>
        /// <returns>The <see cref="FilterConfiguration"/>.</returns>
        public static FilterConfiguration InventoryFilter(this FilterConfiguration filterConfiguration)
        {
            if (filterConfiguration == null)
            {
                return null;
            }

            return filterConfiguration.RangeFacet<VariationContent, double>(
                x => x.TotalInStock().Value,
                (builder, values) => builder.And(x => x.TotalInStock().Value.GreaterThan(values.Min() - 0.1))
                    .And(x => x.TotalInStock().Value.LessThan(values.Max() + 0.1)));
        }
    }
}