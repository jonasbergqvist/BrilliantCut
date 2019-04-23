// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetFilterStore.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Widget.Rest
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using BrilliantCut.Core;
    using BrilliantCut.Core.Service;

    using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
    using EPiServer.Core;
    using EPiServer.Security;
    using EPiServer.Shell.Services.Rest;

    /// <summary>
    /// Class FacetFilterStore.
    /// Implements the <see cref="EPiServer.Shell.Services.Rest.RestControllerBase" />
    /// </summary>
    /// <seealso cref="EPiServer.Shell.Services.Rest.RestControllerBase" />
    [RestStore("facetfilter")]
    public class FacetFilterStore : RestControllerBase
    {
        /// <summary>
        /// The facet service
        /// </summary>
        private readonly FacetService facetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FacetFilterStore"/> class.
        /// </summary>
        /// <param name="facetService">The facet service.</param>
        public FacetFilterStore(FacetService facetService)
        {
            this.facetService = facetService;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="query">The query.</param>
        /// <param name="referenceId">The reference identifier.</param>
        /// <param name="sortColumns">The sort columns.</param>
        /// <param name="range">The range.</param>
        /// <returns>The RestResult.</returns>
        [HttpGet]
        public RestResult Get(
            ContentReference id,
            string query,
            ContentReference referenceId,
            IEnumerable<SortColumn> sortColumns,
            ItemRange range)
        {
            ContentQueryParameters queryParameters = new ContentQueryParameters
                                                         {
                                                             ReferenceId = referenceId,
                                                             SortColumns = sortColumns,
                                                             Range = range,
                                                             AllParameters =
                                                                 this.ControllerContext.HttpContext.Request.QueryString,
                                                             CurrentPrincipal = PrincipalInfo.CurrentPrincipal
                                                         };

            List<FilterContentWithOptions> filterOptions =
                this.facetService.GetItems(parameters: queryParameters).ToList();

            return this.Rest(data: filterOptions);
        }
    }
}