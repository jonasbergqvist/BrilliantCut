using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.ServiceLocation;

namespace EPiTube.FasetFilter.Core
{
    public abstract class FilterContentBase<TContentData, TValueType> : IFilterContent<TContentData>
        where TContentData : IContent
    {
        public abstract string Name { get; }

        public abstract ITypeSearch<TContentData> Filter(IContent currentCntent, ITypeSearch<TContentData> query, IEnumerable<TValueType> values);

        public abstract IDictionary<string, TValueType> GetFilterOptionsFromResult(SearchResults<EPiTubeModel> searchResults);

        public abstract ITypeSearch<TContentData> AddFasetToQuery(ITypeSearch<TContentData> query);
        //public abstract IDictionary<string, TValueType> GetDefaultFilterOptions();

        public virtual string Description
        {
            get { return string.Empty; }
        }

        public ITypeSearch<TContentData> Filter(IContent currentCntent, ISearch query, IEnumerable<object> values)
        {
            return Filter(currentCntent, (ITypeSearch<TContentData>)query, values.Select(x => Convert.ChangeType(x, typeof(TValueType))).Cast<TValueType>());
        }

        public IDictionary<string, object> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        {
            return GetFilterOptionsFromResult(searchResults).ToDictionary<KeyValuePair<string, TValueType>, string, object>(filterOption => filterOption.Key, filterOption => filterOption.Value);
        }

        //public IDictionary<string, object> GetFilterOptions()
        //{
        //    return GetDefaultFilterOptions().ToDictionary<KeyValuePair<string, TValueType>, string, object>(filterOption => filterOption.Key, filterOption => filterOption.Value);
        //}

        public ITypeSearch<TContentData> AddFasetToQuery(ISearch query)
        {
            return AddFasetToQuery((ITypeSearch<TContentData>)query);
        }
    }
}
