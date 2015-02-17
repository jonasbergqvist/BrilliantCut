using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Api.Facets;
using EPiServer.Find.Api.Querying;
using EPiServer.Find.Helpers;
using EPiServer.Find.Helpers.Reflection;

namespace EPiTube.FasetFilter.Core
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

        //public static TermsFacet TermsFacetFor<TContent>(this IHasFacetResults facetsResultsContainer,
        //    Expression<Func<TContent, object>> fieldSelector) where TContent : IContent
        //{
        //    return FacetResultExtraction.TermsFacetFor(facetsResultsContainer, fieldSelector);
        //}

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

        //public static ITypeSearch<TTarget> StatisticalFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //    Expression<Func<TTarget, object>> fieldSelector)
        //{
        //    return StatisticalFacetFor(search, fieldSelector, null);
        //}

        //public static ITypeSearch<TTarget> StatisticalFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //    Expression<Func<TTarget, object>> fieldSelector, Action<StatisticalFacetRequest> requestAction)
        //{
        //    return new Search<TTarget, IQuery>(search, context =>
        //    {
        //        var facetName = String.Format("StatisticalFacet_{0}", fieldSelector.GetFieldPath());
        //        var facetRequest = new StatisticalFacetRequest(facetName);

        //        var fieldName = search.Client.Conventions.FieldNameConvention.GetFieldName(fieldSelector);
        //        facetRequest.Field = fieldName;

        //        if (requestAction != null)
        //        {
        //            requestAction(facetRequest);
        //        }

        //        context.RequestBody.Facets.Add(facetRequest);
        //    });
        //}

        //public static ITypeSearch<TTarget> TermsFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //    Expression<Func<TTarget, IEnumerable<string>>> fieldSelector)
        //{
        //    return search.AddTermsFacetFor<TSource, TTarget>(fieldSelector, null);
        //}

        //public static ITypeSearch<TTarget> TermsFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //    Expression<Func<TTarget, IEnumerable<string>>> fieldSelector, Action<TermsFacetRequest> facetRequestAction)
        //{
        //    return search.AddTermsFacetFor<TSource, TTarget>(fieldSelector, facetRequestAction);
        //}

        //public static ITypeSearch<TTarget> TermsFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //    Expression<Func<TTarget, string>> fieldSelector)
        //{
        //    return search.AddTermsFacetFor<TSource, TTarget>(fieldSelector, null);
        //}

        //public static ITypeSearch<TTarget> TermsFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //    Expression<Func<TTarget, string>> fieldSelector, Action<TermsFacetRequest> facetRequestAction)
        //{
        //    return search.AddTermsFacetFor<TSource, TTarget>(fieldSelector, facetRequestAction);
        //}

        //private static ITypeSearch<TTarget> AddTermsFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //    Expression fieldSelector, Action<TermsFacetRequest> facetRequestAction)
        //{
        //    fieldSelector.ValidateNotNullArgument("fieldSelector");

        //    var facetName = fieldSelector.GetFieldPath();
        //    var fieldName = search.Client.Conventions.FieldNameConvention.GetFieldName(fieldSelector);
        //    var action = facetRequestAction;
        //    return search.TermsFacetFor<TSource, TTarget>(facetName, x =>
        //    {
        //        x.Field = fieldName;
        //        if (action.IsNotNull())
        //        {
        //            action(x);
        //        }
        //    });
        //}

        //public static ITypeSearch<TTarget> TermsFacetFor<TSource, TTarget>(
        //    this ITypeSearch<TSource> search,
        //        string name, Action<TermsFacetRequest> facetRequestAction)
        //{
        //    facetRequestAction.ValidateNotNullArgument("facetRequestAction");
        //    var facetName = name;
        //    var action = facetRequestAction;
        //    return new Search<TTarget, IQuery>(search, context =>
        //    {
        //        var facetRequest = new TermsFacetRequest(facetName);
        //        action(facetRequest);
        //        context.RequestBody.Facets.Add(facetRequest);
        //    });
        //}
    }
}
