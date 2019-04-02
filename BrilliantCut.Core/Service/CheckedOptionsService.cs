// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckedOptionsService.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Service
{
    using System;
    using System.Collections.Generic;

    using BrilliantCut.Core.Models;

    public class CheckedOptionsService
    {
        public FilterModel CreateFilterModel(string checkedOptionsString)
        {
            FilterModel filterModel = new FilterModel { CheckedItems = new Dictionary<string, List<object>>() };
            if (checkedOptionsString == null)
            {
                return filterModel;
            }

            string currentKey = string.Empty;
            string[] items = checkedOptionsString.Split(new[] { "__" }, options: StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < items.Length; i++)
            {
                if (i % 2 == 0)
                {
                    currentKey = items[i].Replace("__", newValue: string.Empty);
                    filterModel.CheckedItems.Add(key: currentKey, value: new List<object>());
                }
                else if (!string.IsNullOrEmpty(value: currentKey))
                {
                    string[] options = items[i].Split(new[] { ".." }, options: StringSplitOptions.RemoveEmptyEntries);
                    foreach (string option in options)
                    {
                        filterModel.CheckedItems[key: currentKey].Add(option.Replace("..", newValue: string.Empty));
                    }
                }
            }

            return filterModel;
        }
    }
}