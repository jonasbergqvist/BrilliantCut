// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterOptionModel.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    public class FilterOptionModel : IFilterOptionModel
    {
        public FilterOptionModel(string id, string text, object value, object defaultValue, int count)
        {
            this.Id = id;
            this.Text = text;
            this.Value = value;
            this.DefaultValue = defaultValue;
            this.Count = count;
        }

        public int Count { get; private set; }

        public object DefaultValue { get; private set; }

        public string Id { get; private set; }

        public string Text { get; private set; }

        public object Value { get; private set; }
    }
}