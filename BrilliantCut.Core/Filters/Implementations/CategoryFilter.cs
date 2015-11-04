using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;

namespace BrilliantCut.Core.Filters.Implementations
{
    public class CategoryFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get { return "Category"; }
        }

        public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<string> values)
        {
            var filter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            filter = values.Aggregate(filter, (current, value) => current.Or(x => x.CategoryNames().MatchCaseInsensitive(value)));

            return query.Filter(filter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode, IContent currentContent)
        {
            var facet = searchResults
                .TermsFacetFor<CatalogContentBase>(x => x.CategoryNames()).Terms;

            return facet.Select(authorCount => new FilterOptionModel("category" + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count));
        }

        public override ITypeSearch<CatalogContentBase> AddfacetToQuery(ITypeSearch<CatalogContentBase> query, FacetFilterSetting setting)
        {
            return query.TermsFacetFor(x => x.CategoryNames(), request =>
            {
                if (setting.MaxFacetHits.HasValue)
                {
                    request.Size = setting.MaxFacetHits;
                }
            });
        }
    }
}
