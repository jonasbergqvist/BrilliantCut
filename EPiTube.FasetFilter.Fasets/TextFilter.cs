using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.ServiceLocation;
using EPiTube.FasetFilter.Core;
using EPiTube.FasetFilter.Core.DataAnnotation;

namespace EPiTube.FasetFilter.Fasets
{
    [ServiceConfiguration, TextboxFilter]
    public class TextFilter : IFilterContent
    {
        private const string ForMethodName = "For";
        public string Name
        {
            get { return "TextSearch"; }
        }

        public string Description
        {
            get { return "Free text search"; }
        }

        public IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        {
            yield return new FilterOptionModel("FreeTextFilter", string.Empty, string.Empty, string.Empty);
        }

        public ISearch Filter(IContent content, ISearch query, IEnumerable<object> values)
        {
            var valueArray = values as string[] ?? values.ToArray();
            if (!valueArray.Any())
            {
                return query;
            }

            var value = valueArray.OfType<string>().First();
            if (String.IsNullOrEmpty(value))
            {
                return query;
            }

            var typeSearchInterface = query.GetType().GetInterface(typeof (ITypeSearch<>).Name);
            if (typeSearchInterface == null)
            {
                return query;
            }

            var genericArgument = typeSearchInterface.GetGenericArguments().First();
            var methodInfoFor = typeof(TypeSearchExtensions).GetMethods().First(x => x.Name == ForMethodName);
            methodInfoFor = methodInfoFor.MakeGenericMethod(genericArgument);
            
            return methodInfoFor.Invoke(null, new object[] { query, value }) as ISearch;
        }

        public ISearch AddFasetToQuery(ISearch query)
        {
            return query;
        }
    }
}