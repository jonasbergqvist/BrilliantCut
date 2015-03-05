using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Api.Querying;
using EPiServer.ServiceLocation;
using EPiTube.FasetFilter.Core.Filters;

namespace EPiTube.FasetFilter.Core
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    public class FilterConfiguration
    {
        private readonly List<IFilterContent> _filters = new List<IFilterContent>();

        public IEnumerable<IFilterContent> Filters { get { return new List<IFilterContent>(_filters); } }

        public FilterConfiguration TermsFaset<TContent>(Expression<Func<TContent, string>> property, Func<FilterBuilder<TContent>, string, FilterBuilder<TContent>> aggregate)
            where TContent : IContent
        {
            var filter = Activator.CreateInstance<TermsFacet<TContent>>();

            filter.PropertyValuesExpression = property;
            filter.Aggregate = aggregate;

            filter.SortOrder = GetSortOrder();
            _filters.Add(filter);

            return this;
        }

        public FilterConfiguration RangeFacet<TContent>(Expression<Func<TContent, double>> property, Func<FilterBuilder<TContent>, IEnumerable<double>, FilterBuilder<TContent>> filterBuilder)
            where TContent : IContent
        {
            var filter = Activator.CreateInstance<RangeFacet<TContent>>();

            filter.PropertyValuesExpression = property;
            filter.FilterBuilder = filterBuilder;
            //filter.Predicate = predicate;
            //filter.GreaterThanFilter = greaterThanFilter;
            //filter.LessThanFilter = lessThanFilter;

            filter.SortOrder = GetSortOrder();
            _filters.Add(filter);

            return this;
        }

        public FilterConfiguration Faset<TFilter>()
            where TFilter : IFilterContent
        {
            var filter = Activator.CreateInstance<TFilter>();
            filter.SortOrder = GetSortOrder();
            _filters.Add(filter);

            return this;
        }

        private int GetSortOrder()
        {
            return _filters.Any() ? _filters.Last().SortOrder + 1 : 1;
        }
    }
}
