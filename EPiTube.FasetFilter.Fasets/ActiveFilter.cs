using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.ServiceLocation;
using EPiTube.FasetFilter.Core;

namespace EPiTube.FasetFilter.Fasets
{
    //[ServiceConfiguration]
    public class IsActiveFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get { return "Active"; }
        }

        public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<string> values)
        {
            if (!values.Any())
            {
                return query;
            }

            return query.CurrentlyPublished().ExcludeDeleted();
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        {
            yield return new FilterOptionModel("OnlyActive", "Only active", false, false);
        }

        public override ITypeSearch<CatalogContentBase> AddFasetToQuery(ITypeSearch<CatalogContentBase> query)
        {
            return query;
        }
    }
}
