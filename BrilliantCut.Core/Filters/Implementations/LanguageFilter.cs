// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Api.Facets;
    using EPiServer.Find.Framework;

    [RadiobuttonFilter]
    public class LanguageFilter : FilterContentBase<CatalogContentBase, string>
    {
        public override string Name
        {
            get
            {
                return "Language";
            }
        }

        public override ITypeSearch<CatalogContentBase> AddFacetToQuery(
            ITypeSearch<CatalogContentBase> query,
            FacetFilterSetting setting)
        {
            return query.TermsFacetFor(
                x => x.LanguageName(),
                request =>
                    {
                        if (setting.MaxFacetHits.HasValue)
                        {
                            request.Size = setting.MaxFacetHits;
                        }
                    });
        }

        public override ITypeSearch<CatalogContentBase> Filter(
            IContent currentCntent,
            ITypeSearch<CatalogContentBase> query,
            IEnumerable<string> values)
        {
            ILocale localizableContent = currentCntent as ILocale;

            string[] valuesArray = values.ToArray();
            if ((!valuesArray.Any() || (valuesArray.Length == 1 && valuesArray[0] == "current"))
                && localizableContent != null)
            {
                return query.Filter(
                    x => x.LanguageName().MatchCaseInsensitive(localizableContent.Language.Name));
            }

            FilterBuilder<ILocale> marketFilter = SearchClient.Instance.BuildFilter<ILocale>();
            marketFilter = valuesArray.Aggregate(
                seed: marketFilter,
                func: (current, value) => current.Or(x => x.LanguageName().Match(value)));

            return query.Filter(filter: marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            IEnumerable<TermCount> facet = searchResults.TermsFacetFor<ILocalizable>(x => x.LanguageName()).Terms;

            ILocale currentLocaleContent = currentContent as ILocale;
            string currentLanguage = currentLocaleContent != null ? currentLocaleContent.Language.Name : string.Empty;
            List<FilterOptionModel> filterOptionModels = new List<FilterOptionModel>
                                                             {
                                                                 new FilterOptionModel(
                                                                     "languageCurrent",
                                                                     "Current",
                                                                     "current",
                                                                     true,
                                                                     facet.Where(x => x.Term == currentLanguage)
                                                                         .Sum(x => x.Count))
                                                             };
            filterOptionModels.AddRange(
                facet.Select(
                    authorCount => new FilterOptionModel(
                        "language" + authorCount.Term,
                        string.Format(
                            provider: CultureInfo.InvariantCulture,
                            format: "{0} ({1})",
                            arg0: authorCount.Term,
                            arg1: authorCount.Count),
                        value: authorCount.Term,
                        defaultValue: false,
                        count: authorCount.Count)));

            return filterOptionModels;
        }
    }
}