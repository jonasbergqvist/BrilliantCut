// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildrenDescendentsFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Cms;

    /// <summary>
    /// Class ChildrenDescendentsFilter.
    /// Implements the <see cref="FilterContentBase{TContentData,TValueType}" />
    /// </summary>
    /// <seealso cref="FilterContentBase{TContentData,TValueType}" />
    [RadiobuttonFilter]
    public class ChildrenDescendentsFilter : FilterContentBase<CatalogContentBase, string>
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return "Deep";
            }
        }

        /// <summary>
        /// Adds the facet to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The <see cref="T:EPiServer.Find.ITypeSearch`1" /> of <see cref="T:EPiServer.Core.IContent" />.</returns>
        public override ITypeSearch<CatalogContentBase> AddFacetToQuery(
            ITypeSearch<CatalogContentBase> query,
            FacetFilterSetting setting)
        {
            return query;
        }

        /// <summary>
        /// Filters the specified current content.
        /// </summary>
        /// <param name="currentContent">The current content.</param>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="ITypeSearch{TSource}" /> of <see cref="CatalogContentBase"/>.</returns>
        public override ITypeSearch<CatalogContentBase> Filter(
            IContent currentContent,
            ITypeSearch<CatalogContentBase> query,
            IEnumerable<string> values)
        {
            if (currentContent is ProductContent)
            {
                return query
                    .Filter(
                        x => x.ParentProducts().MatchContained(y => y.ID, currentContent.ContentLink.ID))
                    .Filter(x => !x.ContentLink.Match(currentContent.ContentLink));
            }

            string[] valueArray = values as string[] ?? values.ToArray();
            if (valueArray.Any() && valueArray.First() == "Children")
            {
                return query.Filter(x => x.ParentLink.Match(currentContent.ContentLink.ToReferenceWithoutVersion()));
            }

            return query.Filter(
                x => x.Ancestors().Match(currentContent.ContentLink.ToReferenceWithoutVersion().ToString()));
        }

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <param name="searchResults">The search results.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="currentContent">The current content.</param>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:BrilliantCut.Core.Models.IFilterOptionModel" />.</returns>
        public override IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            if (mode != ListingMode.WidgetListing)
            {
                yield return new FilterOptionModel("Children", "Children", "Children", true, -1);
            }

            yield return new FilterOptionModel("Descendents", "Descendents", "Descendents", false, -1);
        }
    }
}