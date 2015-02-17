using System.Collections;
using System.Collections.Generic;

namespace EPiTube.FasetFilter.Core
{
    public class EPiTubeModelCollection : IEnumerable<EPiTubeModel>
    {
        private readonly List<EPiTubeModel> _models;
        private readonly List<FilterContentWithOptions> _filters; 

        public EPiTubeModelCollection()
        {
            _filters = new List<FilterContentWithOptions>();
            _models = new List<EPiTubeModel>();
        }

        public void Add(EPiTubeModel model)
        {
            _models.Add(model);
        }

        public void AddRange(IEnumerable<EPiTubeModel> models)
        {
            _models.AddRange(models);
        }

        public int Count { get { return _models.Count; } }

        public void AddFilter(FilterContentWithOptions filter)
        {
            _filters.Add(filter);
        }

        public void AddFilters(IEnumerable<FilterContentWithOptions> filters)
        {
            _filters.AddRange(filters);
        }

        public IEnumerable<FilterContentWithOptions> Filters { get { return _filters; } } 

        public IEnumerator<EPiTubeModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
