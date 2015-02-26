using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Api.Querying.Queries;
using EPiServer.ServiceLocation;
using EPiTube.FasetFilter.Core;
using EPiTube.FasetFilter.Core.DataAnnotation;

namespace EPiTube.FasetFilter.Fasets
{
    [ServiceConfiguration, TextboxFilter]
    public class TextFilter : IFilterContent // FilterContentBase<CatalogContentBase, string>
    {
        private const string SearchMethodName = "Search";

        //public override string Name
        //{
        //    get { return "AFreeTextSearch"; }
        //}

        //public override ITypeSearch<CatalogContentBase> Filter(IContent currentCntent, ITypeSearch<CatalogContentBase> query, IEnumerable<string> values)
        //{
        //    var valueArray = values as string[] ?? values.ToArray();
        //    if (!valueArray.Any())
        //    {
        //        return query;
        //    }

        //    var value = valueArray.First();
        //    if (String.IsNullOrEmpty(value))
        //    {
        //        return query;
        //    }

        //    return query.For(value)
        //        .Include(x => x.Name.AnyWordBeginsWith(value))
        //        .Include(x => x.Code().AnyWordBeginsWith(value));
        //}

        //public override IDictionary<string, string> GetFilterOptionsFromResult(SearchResults<EPiTubeModel> searchResults)
        //{
        //    return new Dictionary<string, string>() { { "FreeTextFilter", string.Empty } };
        //}

        //public override ITypeSearch<CatalogContentBase> AddFasetToQuery(ITypeSearch<CatalogContentBase> query)
        //{
        //    return query;
        //}
        public string Name
        {
            get { return "TextSearch"; }
        }

        public string Description
        {
            get { return "Free text search"; }
        }

        public IDictionary<string, object> GetFilterOptions(SearchResults<EPiTubeModel> searchResults)
        {
            return new Dictionary<string, object>() { { "FreeTextFilter", string.Empty } };
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
            //var methodInfoFor = typeof (TypeSearchExtensions).GetMethod("For", new[] { query.GetType(), typeof(string) });
            var methodInfoFor = typeof (TypeSearchExtensions).GetMethods().Where(x => x.Name == "For").First();
            methodInfoFor = methodInfoFor.MakeGenericMethod(genericArgument);
            
            return methodInfoFor.Invoke(null, new object[] { query, value }) as ISearch;

            //var searchQuery = "*";
            //if (!String.IsNullOrWhiteSpace(value))
            //{
            //    searchQuery = value.Quote();
            //}

            //var queryStringQuery = new QueryStringQuery(searchQuery) { RawQuery = value };

            //var searchContext = new SearchContext();
            //searchContext.RequestBody.Query = queryStringQuery;

            //query.ApplyActions(searchContext);

            //return query;

            // -----------------

            //var searchQuery =
            //    CreateSearchQuery(
            //        query.GetType().GetInterface(typeof(ITypeSearch<>).Name).GetGenericArguments().First(),
            //        query.Client);

            //var valueArray = values as string[] ?? values.ToArray();
            //if (!valueArray.Any())
            //{
            //    return searchQuery;
            //}

            //var value = valueArray.OfType<string>().First();
            //if (String.IsNullOrEmpty(value))
            //{
            //    return searchQuery;
            //}

            //return searchQuery.For(value)
            //    .Include(x => ((CatalogContentBase)x).Name.AnyWordBeginsWith(value))
            //    .Include(x => ((CatalogContentBase)x).Code().AnyWordBeginsWith(value));
        }

        private ITypeSearch<object> CreateSearchQuery(Type contentType, IClient client)
        {
            // Consider another way of creating an instance of the generic search. Invoke is pretty slow.
            var method = typeof(Client).GetMethod(SearchMethodName, Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(contentType);
            return genericMethod.Invoke(client, null) as ITypeSearch<object>;
        }

        public ISearch AddFasetToQuery(ISearch query)
        {
            return query;
        }
    }
}

            //if (search == null)
            //{
            //    throw new ArgumentNullException("search");
            //}

            //var searchQuery = "*";
            //if (!StringIsNullOrWhiteSpace(queryString))
            //{
            //    searchQuery = queryString.Quote();
            //}

            //var result = new Search<TSource, QueryStringQuery>(search, context =>
            //{
            //    var query = new QueryStringQuery(searchQuery) { RawQuery = queryString };

            //    context.RequestBody.Query = query;

            //    if (queryStringQueryAction.IsNotNull())
            //    {
            //        queryStringQueryAction(query);
            //    }
            //});

            //// apply unified weights on UnifiedSearch as it is applied default in the UnifiedSearchFor (see bug #118419)
            //if (typeof (TSource) == typeof (ISearchContent))
            //{
            //    return (IQueriedSearch<TSource, QueryStringQuery>) ((IQueriedSearch<ISearchContent, QueryStringQuery>)result).UsingUnifiedWeights();
            //}

            //return result;