using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using EPiTube.FasetFilter.Core.Filters;
using EPiTube.FasetFilter.Core.Settings;

namespace EPiTube.FasetFilter.Core
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    public class FilterConfiguration
    {
        private readonly Dictionary<IFilterContent, FasetFilterSetting> _filters = new Dictionary<IFilterContent, FasetFilterSetting>();

        public IDictionary<IFilterContent, FasetFilterSetting> Filters { get { return new Dictionary<IFilterContent, FasetFilterSetting>(_filters); } }

        public FilterConfiguration TermsFaset<TContent>(
            Expression<Func<TContent, string>> property,
            Func<FilterBuilder<TContent>, string, FilterBuilder<TContent>> aggregate)
            where TContent : IContent
        {
            return TermsFaset(property, aggregate, null);
        }

        public FilterConfiguration TermsFaset<TContent>(
            Expression<Func<TContent, string>> property, 
            Func<FilterBuilder<TContent>, string, FilterBuilder<TContent>> aggregate,
            FasetFilterSetting setting)
            where TContent : IContent
        {
            var filter = Activator.CreateInstance<TermsFacet<TContent>>();

            filter.PropertyValuesExpression = property;
            filter.Aggregate = aggregate;

            filter.SortOrder = GetSortOrder();
            _filters.Add(filter, setting);

            return this;
        }

        public FilterConfiguration RangeFacet<TContent>(
            Expression<Func<TContent, double>> property,
            Func<FilterBuilder<TContent>, IEnumerable<double>, FilterBuilder<TContent>> filterBuilder)
            where TContent : IContent
        {
            return RangeFacet(property, filterBuilder, null);
        }

        public FilterConfiguration RangeFacet<TContent>(
            Expression<Func<TContent, double>> property, 
            Func<FilterBuilder<TContent>, IEnumerable<double>, FilterBuilder<TContent>> filterBuilder,
            FasetFilterSetting setting)
            where TContent : IContent
        {
            var filter = Activator.CreateInstance<RangeFacet<TContent>>();

            filter.PropertyValuesExpression = property;
            filter.FilterBuilder = filterBuilder;

            filter.SortOrder = GetSortOrder();
            _filters.Add(filter, setting);

            return this;
        }

        public FilterConfiguration Faset<TFilter>()
            where TFilter : IFilterContent
        {
            return Faset<TFilter>(null);
        }

        public FilterConfiguration Faset<TFilter>(FasetFilterSetting setting)
            where TFilter : IFilterContent
        {
            var filter = Activator.CreateInstance<TFilter>();
            filter.SortOrder = GetSortOrder();
            _filters.Add(filter, setting);

            return this;
        }

        private int GetSortOrder()
        {
            return _filters.Any() ? _filters.Keys.Select(x => x.SortOrder).Max() + 1 : 1;
        }
    }
}
