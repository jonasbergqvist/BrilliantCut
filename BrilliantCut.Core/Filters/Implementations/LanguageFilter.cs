using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using BrilliantCut.Core.DataAnnotation;
using BrilliantCut.Core.Extensions;
using BrilliantCut.Core.FilterSettings;
using BrilliantCut.Core.Models;

namespace BrilliantCut.Core.Filters.Implementations
{
    [RadiobuttonFilter]
    public class LanguageFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get { return "Language"; }
        }

        public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<string> values)
        {
            var localizableContent = currentCntent as ILocale;

            var valuesArray = values.ToArray();
            if ((!valuesArray.Any() || (valuesArray.Length == 1 && valuesArray[0] == "current")) && localizableContent != null)
            {
                return query.Filter(x => x.LanguageName().MatchCaseInsensitive(localizableContent.Language.Name));
            }

            var marketFilter = SearchClient.Instance.BuildFilter<ILocale>();
            marketFilter = valuesArray.Aggregate(marketFilter, (current, value) => current.Or(x => x.LanguageName().Match(value)));

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode, IContent currentContent)
        {
            var facet = searchResults
                .TermsFacetFor<ILocalizable>(x => x.LanguageName()).Terms;

            var currentLocaleContent = currentContent as ILocale;
            var currentLanguage = currentLocaleContent != null ? currentLocaleContent.Language.Name : string.Empty;
            var filterOptionModels = new List<FilterOptionModel>
            {
                new FilterOptionModel("languageCurrent", "Current", "current", true, facet.Where(x => x.Term == currentLanguage).Sum(x => x.Count))
            };
            filterOptionModels.AddRange(facet.Select(authorCount => new FilterOptionModel("language" + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count)));

            return filterOptionModels;
        }

        public override ITypeSearch<CatalogContentBase> AddfacetToQuery(ITypeSearch<CatalogContentBase> query, FacetFilterSetting setting)
        {
            return query.TermsFacetFor(x => x.LanguageName(), request =>
            {
                if (setting.MaxFacetHits.HasValue)
                {
                    request.Size = setting.MaxFacetHits;
                }
            });
        }
    }
}