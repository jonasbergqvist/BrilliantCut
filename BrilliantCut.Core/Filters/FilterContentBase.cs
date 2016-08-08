using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Find;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;

namespace BrilliantCut.Core.Filters
{
    public abstract class FilterContentBase<TContentData, TValueType> : IFilterContent
        where TContentData : IContent
    {
        public abstract string Name { get; }

        public abstract ITypeSearch<TContentData> Filter(IContent currentCntent, ITypeSearch<TContentData> query, IEnumerable<TValueType> values);

        public abstract IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<object> searchResults, ListingMode mode, IContent currentContent);

        public abstract ITypeSearch<TContentData> AddfacetToQuery(ITypeSearch<TContentData> query, FacetFilterSetting setting);

        public virtual string Description
        {
            get { return string.Empty; }
        }

        public ISearch Filter(IContent currentCntent, ISearch query, IEnumerable<object> values)
        {
            return Filter(currentCntent, (ITypeSearch<TContentData>)query, values.Select(x => Convert.ChangeType(x, typeof(TValueType))).Cast<TValueType>());
        }

        public ISearch AddfacetToQuery(ISearch query, FacetFilterSetting setting)
        {
            return AddfacetToQuery((ITypeSearch<TContentData>)query, setting);
        }
    }
}
