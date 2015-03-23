using System;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Helpers;
using EPiServer.Find.Helpers.Reflection;

namespace EPiTube.FacetFilter.Core.Extensions
{
    public static class HasFacetResultsExtensions
    {
        public static GeoDistanceFacet GeoDistanceFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, GeoLocation>> fieldSelector) where TContent : IContent
        {
            return FacetResultExtraction.GeoDistanceFacetFor(facetsResultsContainer, fieldSelector);
        }

        public static DateHistogramFacet HistogramFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime?>> fieldSelector) where TContent : IContent
        {
            return FacetResultExtraction.HistogramFacetFor(facetsResultsContainer, fieldSelector);
        }

        public static DateHistogramFacet HistogramFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime>> fieldSelector) where TContent : IContent
        {
            return FacetResultExtraction.HistogramFacetFor(facetsResultsContainer, fieldSelector);
        }

        public static HistogramFacet HistogramFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, object>> fieldSelector) where TContent : IContent
        {
            return FacetResultExtraction.HistogramFacetFor(facetsResultsContainer, fieldSelector);
        }

        public static DateRangeFacet RangeFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime?>> fieldSelector) where TContent : IContent
        {
            return FacetResultExtraction.RangeFacetFor(facetsResultsContainer, fieldSelector);
        }

        public static DateRangeFacet RangeFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, DateTime>> fieldSelector) where TContent : IContent
        {
            return FacetResultExtraction.RangeFacetFor(facetsResultsContainer, fieldSelector);
        }

        public static NumericRangeFacet RangeFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, object>> fieldSelector) where TContent : IContent
        {
            return FacetResultExtraction.RangeFacetFor(facetsResultsContainer, fieldSelector);
        }

        public static StatisticalFacet StatisticalFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
            Expression<Func<TContent, object>> fieldSelector) where TContent : IContent
        {
            return GetFacetFor<StatisticalFacet>(facetsResultsContainer, String.Format("StatisticalFacet_{0}", fieldSelector.GetFieldPath()));
        }

        public static StatisticalFacet StatisticalFacetForSumOf<TContent>(this IHasFacetResults facetsResultsContainer,
            params Expression<Func<TContent, object>>[] fieldSelectors) where TContent : IContent
        {
            if (!fieldSelectors.Any())
            {
                throw new ArgumentException("'fieldSelectors' needs to contain at least one expression.");
            }

            return GetFacetFor<StatisticalFacet>(facetsResultsContainer, String.Format("StatisticalFacet_{0}", String.Join("_", fieldSelectors.Select(x => x.GetFieldPath()))));
        }

        public static TFacet GetFacetFor<TFacet>(IHasFacetResults facetsResultsContainer, string facetName)
            where TFacet : Facet
        {
            var facet = facetsResultsContainer.Facets[facetName];
            if (facet.IsNull())
            {
                return null;
            }

            return facet as TFacet;
        }
    }
}
