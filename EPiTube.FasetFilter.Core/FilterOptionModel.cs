using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.UI.Report;

namespace EPiTube.FasetFilter.Core
{
    public interface IFilterOptionModel//<out TValue>
    {
        string Id { get; }
        string Text { get; }
        object Value { get; }
        object DefaultValue { get; }
        int Count { get; }
    }

    public class FilterOptionModel : IFilterOptionModel //<TValue> : IFilterOptionModel//<TValue>
    {
        public FilterOptionModel(string id, string text, object value, object defaultValue, int count)
        {
            Id = id;
            Text = text;
            Value = value;
            DefaultValue = defaultValue;
            Count = count;
        }

        public string Id { get; private set; }
        public string Text { get; private set; }
        public object Value { get; private set; }
        public object DefaultValue { get; private set; }
        public int Count { get; private set; }
    }
}
