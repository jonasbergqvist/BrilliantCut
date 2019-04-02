// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterContentBase.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Core;
    using EPiServer.Find;

    public abstract class FilterContentBase<TContentData, TValueType> : IFilterContent
        where TContentData : IContent
    {
        public virtual string Description
        {
            get
            {
                return string.Empty;
            }
        }

        public abstract string Name { get; }

        public abstract ITypeSearch<TContentData> AddFacetToQuery(
            ITypeSearch<TContentData> query,
            FacetFilterSetting setting);

        public ISearch AddFacetToQuery(ISearch query, FacetFilterSetting setting)
        {
            return this.AddFacetToQuery((ITypeSearch<TContentData>)query, setting: setting);
        }

        public abstract ITypeSearch<TContentData> Filter(
            IContent currentCntent,
            ITypeSearch<TContentData> query,
            IEnumerable<TValueType> values);

        public ISearch Filter(IContent currentCntent, ISearch query, IEnumerable<object> values)
        {
            return this.Filter(
                currentCntent: currentCntent,
                query: (ITypeSearch<TContentData>)query,
                values: values.Select(x => Convert.ChangeType(value: x, conversionType: typeof(TValueType)))
                    .Cast<TValueType>());
        }

        public abstract IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent);
    }
}