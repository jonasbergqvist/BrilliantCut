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

    /// <summary>
    /// Class TextFilter.
    /// Implements the <see cref="IFilterContent" />
    /// </summary>
    /// <seealso cref="IFilterContent" />
    [TextboxFilter]
    public class TextFilter : IFilterContent
    {
        /// <summary>
        /// For method name
        /// </summary>
        private const string ForMethodName = "For";

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get
            {
                return "Free text search";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return "Text";
            }
        }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        /// <value>The sort order.</value>
        public int SortOrder { get; set; }

        /// <summary>
        /// Adds the facet to query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>The facet search.</returns>
        public ISearch AddFacetToQuery(ISearch query, FacetFilterSetting setting)
        {
            return query;
        }

        /// <summary>
        /// Filters the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="query">The query.</param>
        /// <param name="values">The values.</param>
        /// <returns>The filtered search.</returns>
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

            if (query == null)
            {
                return null;
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

        /// <summary>
        /// Gets the filter options.
        /// </summary>
        /// <param name="searchResults">The search results.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="currentContent">Content of the current.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IFilterOptionModel"/>.</returns>
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

        /// <summary>
        /// Adds the filter expression.
        /// </summary>
        /// <param name="filterExpression">The filter expression.</param>
        /// <param name="genericArgument">The generic argument.</param>
        /// <param name="search">The search.</param>
        /// <returns>The <see cref="ITypeSearch{TSource}" /> of <see cref="CatalogContentBase"/>.</returns>
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