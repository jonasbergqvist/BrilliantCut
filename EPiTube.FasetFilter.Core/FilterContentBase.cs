using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Find;

namespace EPiTube.FasetFilter.Core
{
    public abstract class FilterContentBase<TContentData, TValueType> : IFilterContent
        where TContentData : IContent
    {
        public abstract string Name { get; }

        public abstract ITypeSearch<TContentData> Filter(IContent currentCntent, ITypeSearch<TContentData> query, IEnumerable<TValueType> values);

        //public abstract IEnumerable<IFilterOptionModel> GetFilterOptionsFromResult(SearchResults<EPiTubeModel> searchResults);

        public abstract IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults);

        public abstract ITypeSearch<TContentData> AddFasetToQuery(ITypeSearch<TContentData> query);

        public virtual string Description
        {
            get { return string.Empty; }
        }

        public ISearch Filter(IContent currentCntent, ISearch query, IEnumerable<object> values)
        {
            return Filter(currentCntent, (ITypeSearch<TContentData>)query, values.Select(x => Convert.ChangeType(x, typeof(TValueType))).Cast<TValueType>());
        }

        //public IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        //{
        //    return (IEnumerable<IFilterOptionModel>)GetFilterOptionsFromResult(searchResults);
        //    //return GetFilterOptionsFromResult(searchResults).ToDictionary<KeyValuePair<string, TValueType>, string, object>(filterOption => filterOption.Key, filterOption => filterOption.Value);
        //}

        public ISearch AddFasetToQuery(ISearch query)
        {
            return AddFasetToQuery((ITypeSearch<TContentData>)query);
        }
    }
}
