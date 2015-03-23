using System.Collections;
using System.Collections.Generic;
using EPiTube.facetFilter.Core;

namespace EPiTube.FacetFilter.Core.Models
{
    public class FacetContentCollection : IEnumerable<IFacetContent>
    {
        private readonly List<IFacetContent> _models;
        private readonly List<FilterContentWithOptions> _filters; 

        public FacetContentCollection()
        {
            _filters = new List<FilterContentWithOptions>();
            _models = new List<IFacetContent>();
        }

        public void Add(FacetContent model)
        {
            _models.Add(model);
        }

        public void AddRange(IEnumerable<FacetContent> models)
        {
            _models.AddRange(models);
        }

        public int Count { get { return _models.Count; } }

        public void AddFilter(FilterContentWithOptions filter)
        {
            _filters.Add(filter);
        }

        public IEnumerable<FilterContentWithOptions> Filters { get { return _filters; } }

        public IEnumerator<IFacetContent> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
