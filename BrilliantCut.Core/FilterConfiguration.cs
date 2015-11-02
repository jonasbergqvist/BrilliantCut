using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using BrilliantCut.Core.Filters;
using BrilliantCut.Core.Filters;
using BrilliantCut.Core.FilterSettings;

namespace BrilliantCut.Core
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    public class FilterConfiguration
    {
        private readonly Dictionary<IFilterContent, FacetFilterSetting> _filters = new Dictionary<IFilterContent, FacetFilterSetting>();

        public IDictionary<IFilterContent, FacetFilterSetting> Filters { get { return new Dictionary<IFilterContent, FacetFilterSetting>(_filters); } }

        public FilterConfiguration Termsfacet<TContent>(
            Expression<Func<TContent, string>> property,
            Func<FilterBuilder<TContent>, string, FilterBuilder<TContent>> aggregate)
            where TContent : IContent
        {
            return Termsfacet(property, aggregate, new FacetFilterSetting());
        }

        public FilterConfiguration Termsfacet<TContent>(
            Expression<Func<TContent, string>> property, 
            Func<FilterBuilder<TContent>, string, FilterBuilder<TContent>> aggregate,
            FacetFilterSetting setting)
            where TContent : IContent
        {
            var filter = Activator.CreateInstance<TermsFacet<TContent>>();

            filter.PropertyValuesExpression = property;
            filter.Aggregate = aggregate;

            setting.SortOrder = GetSortOrder(setting);
            _filters.Add(filter, setting);

            return this;
        }

        //public FilterConfiguration Termsfacet<TContent>(
        //    Expression<Func<TContent, IEnumerable<string>>> property,
        //    Func<FilterBuilder<TContent>, IEnumerable<string>, FilterBuilder<TContent>> aggregate)
        //    where TContent : IContent
        //{
        //    return Termsfacet(property, aggregate, new FacetFilterSetting());
        //}

        //public FilterConfiguration Termsfacet<TContent>(
        //    Expression<Func<TContent, IEnumerable<string>>> property,
        //    Func<FilterBuilder<TContent>, IEnumerable<string>, FilterBuilder<TContent>> aggregate,
        //    FacetFilterSetting setting)
        //    where TContent : IContent
        //{
        //    var filter = Activator.CreateInstance<TermsFacet<TContent>>();

        //    filter.PropertyValuesExpression = property;
        //    filter.Aggregate = aggregate;

        //    setting.SortOrder = GetSortOrder(setting);
        //    _filters.Add(filter, setting);

        //    return this;
        //}

        public FilterConfiguration RangeFacet<TContent, TValue>(
            Expression<Func<TContent, TValue>> property,
            Func<FilterBuilder<TContent>, IEnumerable<TValue>, FilterBuilder<TContent>> filterBuilder)
            where TContent : IContent
        {
            return RangeFacet(property, filterBuilder, new FacetFilterSetting());
        }

        public FilterConfiguration RangeFacet<TContent, TValue>(
            Expression<Func<TContent, TValue>> property,
            Func<FilterBuilder<TContent>, IEnumerable<TValue>, FilterBuilder<TContent>> filterBuilder,
            FacetFilterSetting setting)
            where TContent : IContent
        {
            var filter = Activator.CreateInstance<RangeFacet<TContent, TValue>>();

            filter.PropertyValuesExpression = property;
            filter.FilterBuilder = filterBuilder;

            setting.SortOrder = GetSortOrder(setting);
            _filters.Add(filter, setting);

            return this;
        }

        public FilterConfiguration Facet<TFilter>()
            where TFilter : IFilterContent
        {
            return Facet<TFilter>(new FacetFilterSetting());
        }

        public FilterConfiguration Facet<TFilter>(FacetFilterSetting setting)
            where TFilter : IFilterContent
        {
            var filter = Activator.CreateInstance<TFilter>();
            setting.SortOrder = GetSortOrder(setting);
            _filters.Add(filter, setting);

            return this;
        }

        protected virtual int GetSortOrder(FacetFilterSetting setting)
        {
            if (setting.SortOrder > 0)
            {
                return setting.SortOrder;
            }

            return _filters.Any() ? _filters.Values.Select(x => x.SortOrder).Max() + 1 : 1;
        }
    }
}
