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

    [RestStore("facetfilter")]
    public class FacetFilterStore : RestControllerBase
    {
        private readonly FacetService facetService;

        public FacetFilterStore(FacetService facetService)
        {
            this.facetService = facetService;
        }

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