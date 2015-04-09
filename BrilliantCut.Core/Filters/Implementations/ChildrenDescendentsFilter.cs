using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Cms;
using BrilliantCut.Core.DataAnnotation;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;

namespace BrilliantCut.Core.Filters.Implementations
{
    [RadiobuttonFilter]
    public class ChildrenDescendentsFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get { return "Deep"; }
        }

        public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<string> values)
        {
            if (currentCntent is ProductContent)
            {
                return query
                    .Filter(x => x.ProductLinks().MatchContained(y => y.ID, currentCntent.ContentLink.ID))
                    .Filter(x => !x.ContentLink.Match(currentCntent.ContentLink));
            }

            var valueArray = values as string[] ?? values.ToArray();
            if (valueArray.Any() && valueArray.First() == "Descendents")
            {
                return
                    query.Filter(
                        x =>
                            x.Ancestors()
                                .Match(currentCntent.ContentLink.ToReferenceWithoutVersion().ToString()));
            }

            return query.Filter(x => x.ParentLink.Match(currentCntent.ContentLink.ToReferenceWithoutVersion()));
        }

        public override ITypeSearch<CatalogContentBase> AddfacetToQuery(ITypeSearch<CatalogContentBase> query, FacetFilterSetting setting)
        {
            return query;
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode)
        {
            if (mode == ListingMode.MainListing)
            {
                yield return new FilterOptionModel("Children", "Children", "Children", true, -1);
            }

            yield return new FilterOptionModel("Descendents", "Descendents", "Descendents", false, -1);
        }
    }
}