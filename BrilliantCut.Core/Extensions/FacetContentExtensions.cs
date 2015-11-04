using System;
using System.Collections.Generic;
using BrilliantCut.Core.Models;
using BrilliantCut.Core.Service;
using EPiServer.Commerce.SpecializedProperties;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace BrilliantCut.Core.Extensions
{
    public static class FacetContentExtensions
    {
        public static IEnumerable<ContentReference> ProductLinks(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        public static IEnumerable<ContentReference> VariationLinks(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        public static IEnumerable<ContentReference> NodeLinks(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        public static IEnumerable<Price> Prices(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }

        public static IEnumerable<Inventory> Inventories(this IFacetContent content)
        {
            throw new NotSupportedException("This is only for projection");
        }
    }
}
