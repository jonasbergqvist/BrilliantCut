using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Find;
using EPiTube.facetFilter.Core;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Filters
{
    public abstract class FilterContentBase<TContentData, TValueType> : IFilterContent
        where TContentData : IContent
    {
        public abstract string Name { get; }

        public abstract ITypeSearch<TContentData> Filter(IContent currentCntent, ITypeSearch<TContentData> query, IEnumerable<TValueType> values);

        public abstract IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<FacetContent> searchResults);

        public abstract ITypeSearch<TContentData> AddfacetToQuery(ITypeSearch<TContentData> query);

        public virtual string Description
        {
            get { return string.Empty; }
        }

        public ISearch Filter(IContent currentCntent, ISearch query, IEnumerable<object> values)
        {
            return Filter(currentCntent, (ITypeSearch<TContentData>)query, values.Select(x => Convert.ChangeType(x, typeof(TValueType))).Cast<TValueType>());
        }

        public ISearch AddfacetToQuery(ISearch query)
        {
            return AddfacetToQuery((ITypeSearch<TContentData>)query);
        }

        public int SortOrder { get; set; }
    }
}
