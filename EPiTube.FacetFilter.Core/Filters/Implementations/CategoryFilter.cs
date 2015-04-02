using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Filters.Implementations
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
            filter = values.Aggregate(filter, (current, value) => current.Or(x => x.Categories().Match(value)));

            return query.Filter(filter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults)
        {
            var facet = searchResults
                .TermsFacetFor<CatalogContentBase>(x => x.Categories()).Terms;

            return facet.Select(authorCount => new FilterOptionModel("category" + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count));
        }

        public override ITypeSearch<CatalogContentBase> AddfacetToQuery(ITypeSearch<CatalogContentBase> query)
        {
            return query.TermsFacetFor(x => x.Categories(), request => request.Size = 100);
        }
    }
}
