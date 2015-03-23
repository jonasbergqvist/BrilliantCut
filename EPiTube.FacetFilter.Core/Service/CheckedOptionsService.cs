using System;
using System.Collections.Generic;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Service
{
    public class CheckedOptionsService
    {
        public FilterModel CreateFilterModel(string checkedOptionsString)
        {
            if (checkedOptionsString == null)
            {
                return new FilterModel();
            }

            var filterModel = new FilterModel() { CheckedItems = new Dictionary<string, List<object>>() };

            var currentKey = string.Empty;
            var items = checkedOptionsString.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < items.Length; i++)
            {
                if (i % 2 == 0)
                {
                    currentKey = items[i].Replace("__", string.Empty);
                    filterModel.CheckedItems.Add(currentKey, new List<object>());
                }
                else if (!String.IsNullOrEmpty(currentKey))
                {
                    var options = items[i].Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var option in options)
                    {
                        filterModel.CheckedItems[currentKey].Add(option.Replace("..", string.Empty));
                    }
                }
            }

            return filterModel;
        }
    }
}
