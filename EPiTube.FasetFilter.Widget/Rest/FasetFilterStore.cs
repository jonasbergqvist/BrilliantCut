using System.Linq;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;
using EPiTube.FasetFilter.Core;

namespace EPiTube.FasetFilter.Widget.Rest
{
    [RestStore("fasetfilter")]
    public class FasetFilterStore : RestControllerBase
    {
        private readonly FilterContentFactory _filterContentFactory;

        public FasetFilterStore(FilterContentFactory filterContentFactory)
        {
            _filterContentFactory = filterContentFactory;
        }

        [HttpGet]
        public RestResult Get(ContentReference id)
        {
            var filterOptions = _filterContentFactory.GetFilters(id).ToList();
            return Rest(filterOptions);
        }
    }
}