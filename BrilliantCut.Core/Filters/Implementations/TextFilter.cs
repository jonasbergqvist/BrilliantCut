// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextFilter.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using BrilliantCut.Core.DataAnnotation;
    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.FilterSettings;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find;
    using EPiServer.Find.Api.Querying;

    [TextboxFilter]
    public class TextFilter : IFilterContent
    {
        private const string ForMethodName = "For";

        public string Description
        {
            get
            {
                return "Free text search";
            }
        }

        public string Name
        {
            get
            {
                return "Text";
            }
        }

        public int SortOrder { get; set; }

        public ISearch AddFacetToQuery(ISearch query, FacetFilterSetting setting)
        {
            return query;
        }

        public ISearch Filter(IContent content, ISearch query, IEnumerable<object> values)
        {
            object[] valueArray = values as string[] ?? values.ToArray();
            if (!valueArray.Any())
            {
                return query;
            }

            string value = valueArray.OfType<string>().First();
            if (string.IsNullOrEmpty(value: value))
            {
                return query;
            }

            Type typeSearchInterface = query.GetType().GetInterface(name: typeof(ITypeSearch<>).Name);
            if (typeSearchInterface == null)
            {
                return query;
            }

            Type genericArgument = typeSearchInterface.GetGenericArguments().First();
            MethodInfo methodInfoFor = typeof(TypeSearchExtensions).GetMethods().First(x => x.Name == ForMethodName);
            methodInfoFor = methodInfoFor.MakeGenericMethod(genericArgument);

            ITypeSearch<CatalogContentBase> search =
                methodInfoFor.Invoke(null, new object[] { query, value }) as ITypeSearch<CatalogContentBase>;

            Expression<Func<CatalogContentBase, Filter>> nameFilterExpression =
                x => x.Name.AnyWordBeginsWith(value);
            search = AddFilterExpression(
                filterExpression: nameFilterExpression,
                genericArgument: genericArgument,
                search: search);

            Expression<Func<CatalogContentBase, Filter>> codeFilterExpression =
                x => x.Code().AnyWordBeginsWith(value);
            search = AddFilterExpression(
                filterExpression: codeFilterExpression,
                genericArgument: genericArgument,
                search: search);

            return search;
        }

        public IEnumerable<IFilterOptionModel> GetFilterOptions(
            SearchResults<object> searchResults,
            ListingMode mode,
            IContent currentContent)
        {
            yield return new FilterOptionModel(
                "FreeTextFilter",
                text: string.Empty,
                value: string.Empty,
                defaultValue: string.Empty,
                count: -1);
        }

        private static ITypeSearch<CatalogContentBase> AddFilterExpression(
            Expression<Func<CatalogContentBase, Filter>> filterExpression,
            Type genericArgument,
            ITypeSearch<CatalogContentBase> search)
        {
            Type delegateType = typeof(Func<,>).MakeGenericType(genericArgument, typeof(Filter));
            LambdaExpression lambdaExpression = Expression.Lambda(
                delegateType,
                filterExpression.Body,
                filterExpression.Parameters[0]);

            MethodInfo methodInfoInclude = typeof(TypeSearchExtensions).GetMethods().First(x => x.Name == "Include");
            methodInfoInclude = methodInfoInclude.MakeGenericMethod(genericArgument);
            return methodInfoInclude.Invoke(null, new object[] { search, lambdaExpression, null }) as
                       ITypeSearch<CatalogContentBase>;
        }
    }
}