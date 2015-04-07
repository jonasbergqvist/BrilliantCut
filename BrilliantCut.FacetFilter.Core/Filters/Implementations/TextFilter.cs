using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Api.Querying;
using BrilliantCut.FacetFilter.Core.DataAnnotation;
using BrilliantCut.FacetFilter.Core.Extensions;
using BrilliantCut.FacetFilter.Core.FilterSettings;
using BrilliantCut.FacetFilter.Core.Models;

namespace BrilliantCut.FacetFilter.Core.Filters.Implementations
{
    [TextboxFilter]
    public class TextFilter : IFilterContent
    {
        private const string ForMethodName = "For";
        public string Name
        {
            get { return "Text"; }
        }

        public string Description
        {
            get { return "Free text search"; }
        }

        public IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode)
        {
            yield return new FilterOptionModel("FreeTextFilter", string.Empty, string.Empty, string.Empty, -1);
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

            var typeSearchInterface = query.GetType().GetInterface(typeof(ITypeSearch<>).Name);
            if (typeSearchInterface == null)
            {
                return query;
            }

            var genericArgument = typeSearchInterface.GetGenericArguments().First();
            var methodInfoFor = typeof(TypeSearchExtensions).GetMethods().First(x => x.Name == ForMethodName);
            methodInfoFor = methodInfoFor.MakeGenericMethod(genericArgument);

            var search = methodInfoFor.Invoke(null, new object[] { query, value }) as ITypeSearch<CatalogContentBase>;

            Expression<Func<CatalogContentBase, Filter>> nameFilterExpression = (x) => x.Name.AnyWordBeginsWith(value);
            search = AddFilterExpression(nameFilterExpression, genericArgument, search);

            Expression<Func<CatalogContentBase, Filter>> codeFilterExpression = (x) => x.Code().AnyWordBeginsWith(value);
            search = AddFilterExpression(codeFilterExpression, genericArgument, search);

            return search;
        }

        private static ITypeSearch<CatalogContentBase> AddFilterExpression(Expression<Func<CatalogContentBase, Filter>> filterExpression, Type genericArgument, ITypeSearch<CatalogContentBase> search)
        {
            var delegateType = typeof(Func<,>).MakeGenericType(genericArgument, typeof(Filter));
            var lambdaExpression = Expression.Lambda(delegateType, filterExpression.Body, filterExpression.Parameters[0]);

            var methodInfoInclude = typeof(TypeSearchExtensions).GetMethods().First(x => x.Name == "Include");
            methodInfoInclude = methodInfoInclude.MakeGenericMethod(genericArgument);
            return methodInfoInclude.Invoke(null, new object[] { search, lambdaExpression, null }) as ITypeSearch<CatalogContentBase>;
        }

        public ISearch AddfacetToQuery(ISearch query, FacetFilterSetting setting)
        {
            return query;
        }

        public int SortOrder { get; set; }
    }
}