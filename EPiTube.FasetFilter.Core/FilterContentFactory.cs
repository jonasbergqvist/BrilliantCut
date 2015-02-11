using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Find;
using EPiServer.Find.Cms;
using EPiServer.Find.Framework;
using EPiServer.Framework.Cache;
using EPiServer.Framework.TypeScanner;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Services.Rest;

namespace EPiTube.FasetFilter.Core
{
    [ServiceConfiguration(typeof(FilterContentFactory))]
    public class FilterContentFactory
    {
        private const int MaxItems = 900;
        private const string SearchMethodName = "Search";

        private IClient _client;
        private readonly Lazy<IEnumerable<FilterContentModelType>> _filterContentsWithGenericTypes;
        private readonly ITypeScannerLookup _typeScannerLookup;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ISynchronizedObjectInstanceCache _synchronizedObjectInstanceCache;

        public FilterContentFactory(
            ITypeScannerLookup typeScannerLookup, 
            IContentTypeRepository contentTypeRepository,
            IContentRepository contentRepository,
            ISynchronizedObjectInstanceCache synchronizedObjectInstanceCache)
        {
            _typeScannerLookup = typeScannerLookup;
            _contentTypeRepository = contentTypeRepository;
            _contentRepository = contentRepository;
            _synchronizedObjectInstanceCache = synchronizedObjectInstanceCache;

            _filterContentsWithGenericTypes = new Lazy<IEnumerable<FilterContentModelType>>(FilterContentsWithGenericTypesValueFactory);
        }

        public IClient Client { get { return _client ?? (_client = SearchClient.Instance); } }

        public IEnumerable<FilterContentWithOptions> GetFilters(ContentReference contentLink)
        {
            var cacheKey = "EPiTube:GetFilters" + contentLink;
            var filters = _synchronizedObjectInstanceCache.Get(cacheKey) as IEnumerable<FilterContentWithOptions>;
            if (filters != null)
            {
                return filters;
            }

            var contentTypes = GetContentTypes(contentLink);

            var supportedFilters = new List<FilterContentModelType>();
            foreach (var supportedType in contentTypes)
            {
                supportedFilters.AddRange(
                    _filterContentsWithGenericTypes.Value
                    .Where(x => 
                        x.ContentType.IsAssignableFrom(supportedType) && 
                        !supportedFilters.Select(y => y.Filter.Name).Contains(x.Filter.Name)));
            }

            filters = supportedFilters
                .Select(x => new FilterContentWithOptions()
                {
                    FilterContent = x.Filter,
                    FilterOptions = x.Filter.GetFilterOptions(contentLink).ToArray()
                }).OrderBy(x => x.FilterContent.Name).ToList();

            _synchronizedObjectInstanceCache.Insert(
                cacheKey, 
                filters, 
                new CacheEvictionPolicy(null, null, new []
                {
                    DataFactoryCache.RootKeyName
                }, 
                new TimeSpan(1, 0, 0), 
                CacheTimeoutType.Sliding));

            return filters;
        }

        protected virtual IEnumerable<Type> GetContentTypes(ContentReference contentLink)
        {
            var cacheKey = "EPiTube:ChildTypes" + contentLink;
            var childTypes = _synchronizedObjectInstanceCache.Get(cacheKey) as IEnumerable<Type>;

            if (childTypes == null)
            {
                var typeIds = SearchClient.Instance.Search<IContent>()
                            .Filter(x => x.Ancestors().Match(contentLink.ToReferenceWithoutVersion().ToString()))
                            .Select(x => x.ContentTypeID)
                            .GetResult();

                childTypes = _contentTypeRepository.List().Where(x => typeIds.Contains(x.ID)).Select(x => x.ModelType);
                _synchronizedObjectInstanceCache.Insert(cacheKey, childTypes, new CacheEvictionPolicy(null, null, new[] { DataFactoryCache.RootKeyName }));
            }

            return childTypes;
        }

        private ITypeSearch<TSource> OrderBy<TSource, TProperty>(
            ITypeSearch<TSource> search,
            Expression<Func<TSource, TProperty>> fieldSelector)
        {
            return search.OrderBy(fieldSelector);
        }

        public IEnumerable<EPiTubeModel> GetFilteredChildren(
            IContent content,
            Type contentType,
            IDictionary<string, IEnumerable<object>> values,
            bool productGrouped,
            SortColumn sortColumn,
            ItemRange range)
        {
            var includeProductVariationRelations = productGrouped && !(content is ProductContent);
            var query = CreateSearchQuery(contentType);

            var startIndex = range.Start ?? 0;
            var endIndex = includeProductVariationRelations ? MaxItems : range.End ?? MaxItems;

            var cacheKey = content.ContentLink.ToString();
            if (!String.IsNullOrEmpty(sortColumn.ColumnName))
            {
                cacheKey += sortColumn.ColumnName + sortColumn.SortDescending;
            }
            if (!includeProductVariationRelations)
            {
                cacheKey += startIndex + endIndex;
            }
            
            foreach (var supportedFilter in SupportedFilters(contentType))
            {
                var filterValues = (values.ContainsKey(supportedFilter.Name)
                                       ? values[supportedFilter.Name]
                                       : Enumerable.Empty<object>()).ToArray();

                query = supportedFilter.Filter(content, query, filterValues);

                cacheKey += String.Join(";", filterValues);
            }

            query = Sort(sortColumn, query);

            var cachedItems = GetCachedContent(cacheKey);
            if (cachedItems != null)
            {
                range.Total = cachedItems.Item2;
                return cachedItems.Item1;
            }

            var contentList = new List<EPiTubeModel>();
            var linkedProductLinks = new List<ContentReference>();

            var total = AddFilteredChildren(query, contentList, linkedProductLinks, includeProductVariationRelations, startIndex, endIndex);
            range.Total = includeProductVariationRelations ? contentList.Count : total;

            Cache(cacheKey, new Tuple<List<EPiTubeModel>, int>(contentList, total));
            return contentList;
        }

        private static ITypeSearch<CatalogContentBase> Sort(SortColumn sortColumn, ITypeSearch<CatalogContentBase> query)
        {
            if (String.IsNullOrEmpty(sortColumn.ColumnName))
            {
                return query;
            }

            switch (sortColumn.ColumnName)
            {
                case "name":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                }
                case "code":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.Code()) : query.OrderBy(x => x.Code());
                }
                case "isPendingPublish":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status);
                }
                case "startPublish":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.StartPublish) : query.OrderBy(x => x.StartPublish);
                }
                case "stopPublish":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.StopPublish) : query.OrderBy(x => x.StopPublish);
                }
                case "metaClassName":
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.MetaClassId()) : query.OrderBy(x => x.MetaClassId());
                }
                default:
                {
                    return sortColumn.SortDescending ? query.OrderByDescending(x => x.ContentTypeID) : query.OrderBy(x => x.ContentTypeID);
                }
            }
        }

        private Tuple<List<EPiTubeModel>, int> GetCachedContent(string cacheKey)
        {
            return _synchronizedObjectInstanceCache.Get(cacheKey) as Tuple<List<EPiTubeModel>, int>;
        }

        private void Cache(string cacheKey, Tuple<List<EPiTubeModel>, int> result)
        {
            _synchronizedObjectInstanceCache.Insert(
                cacheKey, 
                result, 
                new CacheEvictionPolicy(null, null, new[]
                {
                    DataFactoryCache.RootKeyName, 
                    "EP:CatalogKeyPricesMasterCacheKey", 
                    "Mediachase.Commerce.InventoryService.Storage$MASTER"
                }, 
                new TimeSpan(1, 0, 0), 
                CacheTimeoutType.Sliding));
        }

        private int AddFilteredChildren(
            ITypeSearch<CatalogContentBase> query, 
            List<EPiTubeModel> contentList,
            List<ContentReference> linkedProductLinks,
            bool includeProductVariationRelations, 
            int startIndex, 
            int take)
        {
            try
            {
                var queryResult = query
                .Select(x => new
                {
                    x.Name,
                    x.ContentGuid,
                    x.ContentLink,
                    x.IsDeleted,
                    //StartPublishedNormalized = x.StartPublishedNormalized(),
                    ////LanguageName = x.LanguageName(),
                    VariationLinks = x.VariationLinks(),
                    x.ParentLink,
                    x.StartPublish,
                    x.StopPublish,
                    Code = x.Code(),
                    Price = x.DefaultPrice(),
                    x.ContentTypeID,
                    x.ApplicationId,
                    MetaClassId = x.MetaClassId(),
                    ProductLinks = x.ProductLinks(),
                    NodeLinks = x.NodeLinks(),
                    ThumbnailPath = x.ThumbnailPath(),
                    DefaultCurrency = x.DefaultCurrency(),
                    WeightBase = x.WeightBase(),
                    LengthBase = x.LengthBase(),
                    Prices = x.Prices(),
                    Inventires = x.Inventories()
                })
                .Skip(startIndex)
                .Take(take + 2)
                .GetResult();

                var total = queryResult.TotalMatching;
                foreach (var resultItem in queryResult)
                {
                    // When we ask for relations, add product links to linkedProductList if any exists, and do not add a model for the content if it has product links.
                    if (includeProductVariationRelations)
                    {
                        if (resultItem.ProductLinks != null && resultItem.ProductLinks.Any())
                        {
                            foreach (var productLink in resultItem.ProductLinks)
                            {
                                if (!linkedProductLinks.Contains(productLink))
                                {
                                    linkedProductLinks.Add(productLink);
                                }
                            }

                            continue;
                        }
                    }

                    // Add model for content
                    var model = _contentRepository.GetDefault<EPiTubeModel>(resultItem.ParentLink);
                    model.Initialize(resultItem.NodeLinks, resultItem.ProductLinks, resultItem.VariationLinks,
                        resultItem.Prices, resultItem.Inventires);

                    model.Name = resultItem.Name;
                    model.ContentGuid = resultItem.ContentGuid;
                    model.ContentLink = resultItem.ContentLink;
                    model.IsDeleted = resultItem.IsDeleted;
                    model.ParentLink = resultItem.ParentLink;
                    model.StartPublish = resultItem.StartPublish;
                    model.StopPublish = resultItem.StopPublish;
                    model.ContentTypeId = resultItem.ContentTypeID;
                    model.MetaClassId = resultItem.MetaClassId;
                    model.Code = resultItem.Code;
                    model.ApplicationId = resultItem.ApplicationId;
                    model.ThumbnailPath = resultItem.ThumbnailPath;
                    model.HasChildren = resultItem.VariationLinks != null && resultItem.VariationLinks.Any();
                    model.DefaultPrice = resultItem.Price;

                    model.WeightBase = resultItem.WeightBase;
                    model.LengthBase = resultItem.LengthBase;
                    model.DefaultCurrency = resultItem.DefaultCurrency;

                    contentList.Add(model);
                }

                if (includeProductVariationRelations)
                {
                    // get parent products in the query result
                    var addedContentLinks = contentList.Select(x => x.ContentLink);
                    var links = addedContentLinks;
                    var notAddedLinkedProducts = linkedProductLinks.Where(x => !links.Contains(x));
                    foreach (var linkedProductLink in notAddedLinkedProducts)
                    {
                        var resultItem =
                            queryResult.FirstOrDefault(x => x.ContentLink.CompareToIgnoreWorkID(linkedProductLink));
                        if (resultItem != null)
                        {
                            var model = _contentRepository.GetDefault<EPiTubeModel>(resultItem.ParentLink);
                            model.Initialize(resultItem.NodeLinks, resultItem.ProductLinks, resultItem.VariationLinks,
                                resultItem.Prices, resultItem.Inventires);

                            model.Name = resultItem.Name;
                            model.ContentGuid = resultItem.ContentGuid;
                            model.ContentLink = resultItem.ContentLink;
                            model.IsDeleted = resultItem.IsDeleted;
                            model.ParentLink = resultItem.ParentLink;
                            model.StartPublish = resultItem.StartPublish;
                            model.StopPublish = resultItem.StopPublish;
                            model.ContentTypeId = resultItem.ContentTypeID;
                            model.MetaClassId = resultItem.MetaClassId;
                            model.Code = resultItem.Code;
                            model.ApplicationId = resultItem.ApplicationId;
                            model.ThumbnailPath = resultItem.ThumbnailPath;
                            model.HasChildren = resultItem.VariationLinks != null && resultItem.VariationLinks.Any();
                            model.DefaultPrice = resultItem.Price;

                            model.WeightBase = resultItem.WeightBase;
                            model.LengthBase = resultItem.LengthBase;
                            model.DefaultCurrency = resultItem.DefaultCurrency;

                            contentList.Add(model);
                        }
                    }

                    addedContentLinks = contentList.Select(x => x.ContentLink);
                    notAddedLinkedProducts = linkedProductLinks.Where(x => !addedContentLinks.Contains(x)).ToArray();
                    if (notAddedLinkedProducts.Any())
                    {
                        var filterBuilder = new FilterBuilder<ProductContent>(SearchClient.Instance);
                        filterBuilder = notAddedLinkedProducts.Aggregate(filterBuilder,
                            (current, reference) => current.Or(x => x.ContentLink.Match(reference)));

                        var productQuery = SearchClient.Instance.Search<ProductContent>().Filter(filterBuilder);
                        total += AddFilteredChildren(productQuery, contentList, linkedProductLinks, false, 0, MaxItems);
                    }
                }

                return total;   
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }

        public IEnumerable<GetChildrenReferenceResult> GetFilteredChildrenReferences(IContent currentCntent, Type contentType, IDictionary<string, IEnumerable<object>> values, Func<ITypeSearch<CatalogContentBase>, ITypeSearch<CatalogContentBase>> typeSearchModifyerFunc, bool? isLeafNode)
        {
            var query = CreateSearchQuery(contentType);
            foreach (var supportedFilter in SupportedFilters(contentType))
            {
                var filterValues = values.ContainsKey(supportedFilter.Name)
                                       ? values[supportedFilter.Name]
                                       : Enumerable.Empty<object>();

                query = supportedFilter.Filter(currentCntent, query, filterValues);
            }

            query = typeSearchModifyerFunc(query);

            // Here we want to return prices and inventories if the returned object is an variation. How can we do that?
            var queryResult = query.Select(x => new { x.ContentLink, x.ContentTypeID }).GetResult();

            var contentRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            return
                queryResult.Select(
                    x => new GetChildrenReferenceResult() { ContentLink = x.ContentLink, ModelType = contentRepository.Load(x.ContentTypeID).ModelType, IsLeafNode = isLeafNode });
            
        }

        public Type GetSearchType(FilterModel filterModel)
        {
            Type selectedType = null;
            foreach (var filter in filterModel.Value)
            {
                if (!filter.Value.Any())
                {
                    continue;
                }

                var filterContentModelType = _filterContentsWithGenericTypes.Value
                    .SingleOrDefault(x => x.Filter.Name == filter.Name);

                if (filterContentModelType == null)
                {
                    continue;
                }

                if(selectedType == null || selectedType.IsAssignableFrom(filterContentModelType.ContentType))
                {
                    selectedType = filterContentModelType.ContentType;
                }
            }

            return selectedType;
        }

        private ITypeSearch<CatalogContentBase> CreateSearchQuery(Type contentType)
        {
            // Consider another way of creating an instance of the generic search. Invoke is pretty slow.
            var method = typeof(Client).GetMethod(SearchMethodName, Type.EmptyTypes);
            var genericMethod = method.MakeGenericMethod(contentType);
            return genericMethod.Invoke(Client, null) as ITypeSearch<CatalogContentBase>;
        }

        private IEnumerable<IFilterContent<CatalogContentBase>> SupportedFilters(Type queryType)
        {
            var supportedTypes = _filterContentsWithGenericTypes.Value.Where(x => x.ContentType.IsAssignableFrom(queryType)).ToArray(); //.OrderBy(x => x.Filter.Name)
            for (var i = 0; i < supportedTypes.Length; i++)
            {
                for (var j = i; j < supportedTypes.Length; j++)
                {
                    if (supportedTypes[i].ContentType.IsAssignableFrom(supportedTypes[j].ContentType))
                    {
                        var temp = supportedTypes[i];
                        supportedTypes[i] = supportedTypes[j];
                        supportedTypes[j] = temp;
                    }
                }
            }

            return supportedTypes.Select(x => x.Filter);
        }

        private IEnumerable<FilterContentModelType> FilterContentsWithGenericTypesValueFactory()
        {
            foreach (var filterContentType in _typeScannerLookup.AllTypes.Where(x => typeof(IFilterContent).IsAssignableFrom(x)))
            {
                // TODO: Replace activator.CreateInstance with fast lambdaexpression with precompilation and cache
                var filterContent = Activator.CreateInstance(filterContentType) as IFilterContent<CatalogContentBase>;
                yield return new FilterContentModelType { Filter = filterContent, ContentType = filterContentType.GetInterface(typeof(IFilterContent<>).Name).GetGenericArguments()[0] };
            }
        }
    }
}

//if (includeProductVariationRelations)
//{
//    var hits = total;
//    while (endIndex < 1000) //  && hits > 0 && contentList.Count < (endIndex - startIndex)
//    {
//        startIndex += MaxItems;
//        endIndex += MaxItems;

//        hits = AddFilteredChildren(query, contentList, linkedProductLinks, includeProductVariationRelations,
//            startIndex, endIndex);
//    }
//}


//foreach (var resultItem in queryResult)
//{
//    if (contentList.Select(x => x.ContentLink).Contains(resultItem.ContentLink))
//    {
//        continue;
//    }

//    if (includeProductVariationRelations)
//    {
//        var type = _contentTypeRepository.Load(resultItem.ContentTypeID).ModelType;
//        if (resultItem.ProductLinks != null && resultItem.ProductLinks.Any())
//        {
//            if (typeof(VariationContent).IsAssignableFrom(type))
//            {
//                linkedProductLinks.AddRange(resultItem.ProductLinks);
//            }
//        }
//    }
//}

//var productLinksToReceive = linkedProductLinks.Except(queryResult.Select(x => x.ContentLink)).ToList();
//if (productLinksToReceive.Any())
//{
//    var filterBuilder = new FilterBuilder<ProductContent>(SearchClient.Instance);
//    filterBuilder = productLinksToReceive.Aggregate(filterBuilder, (current, reference) => current.Or(x => x.ContentLink.Match(reference)));

//    var productQuery = SearchClient.Instance.Search<ProductContent>()
//        .Filter(filterBuilder);

//    contentList.AddRange(GetFilteredChildren(productQuery, contentList, linkedProductLinks, false, 0, null));
//}

//foreach (var resultItem in queryResult)
//{
//    var contentLinks = contentList.Select(x => x.ContentLink);
//    if (resultItem.ProductLinks != null && resultItem.ProductLinks.All(contentLinks.Contains))
//    {
//        continue;
//    }

//    var model = _contentRepository.GetDefault<EPiTubeModel>(resultItem.ParentLink);
//    model.Initialize(resultItem.NodeLinks, resultItem.ProductLinks, resultItem.VariationLinks, resultItem.Prices, resultItem.Inventires);

//    model.Name = resultItem.Name;
//    model.ContentGuid = resultItem.ContentGuid;
//    model.ContentLink = resultItem.ContentLink;
//    model.IsDeleted = resultItem.IsDeleted;
//    model.ParentLink = resultItem.ParentLink;
//    model.StartPublish = resultItem.StartPublish;
//    model.StopPublish = resultItem.StopPublish;
//    model.ContentTypeId = resultItem.ContentTypeID;
//    model.MetaClassId = resultItem.MetaClassId;
//    model.Code = resultItem.Code;
//    model.ApplicationId = resultItem.ApplicationId;
//    model.ThumbnailPath = resultItem.ThumbnailPath;
//    model.HasChildren = resultItem.VariationLinks != null && resultItem.VariationLinks.Any();
//    model.DefaultPrice = resultItem.Price;

//    model.WeightBase = resultItem.WeightBase;
//    model.LengthBase = resultItem.LengthBase;
//    model.DefaultCurrency = resultItem.DefaultCurrency;

//    contentList.Add(model);
//}

//return contentList;

// --------------

//var variationLinksToReceive = linkedVariationLinks.Except(contentList.Select(x => x.ContentLink)).ToList();
//if (variationLinksToReceive.Any())
//{
//    var filterBuilder = new FilterBuilder<VariationContent>(SearchClient.Instance);
//    filterBuilder = productLinksToReceive.Aggregate(filterBuilder,
//        (filter, link) => filter.Or(x => x.ContentLink.Match(link)));

//    var variationQuery = SearchClient.Instance.Search<VariationContent>()
//        .Filter(filterBuilder);

//    contentList.AddRange(GetFilteredChildren(variationQuery, false, 0));
//}


//var type = _contentTypeRepository.Load(resultItem.ContentTypeID).ModelType;
////var catalogContentBase = ServiceLocator.Current.GetInstance(type) as CatalogContentBase;

//var getDefaultMethod = contentRepositoryType.GetMethod("GetDefault", new[] { typeof(ContentReference) }).MakeGenericMethod(type);
//var catalogContentBase = getDefaultMethod.Invoke(_contentRepository, new object[] { resultItem.ParentLink }) as CatalogContentBase;

//if (catalogContentBase == null)
//{
//    continue;
//}

//catalogContentBase.Name = resultItem.Name;
//catalogContentBase.ContentGuid = resultItem.ContentGuid;
//catalogContentBase.ContentLink = resultItem.ContentLink;
//catalogContentBase.IsDeleted = resultItem.IsDeleted;
//catalogContentBase.ParentLink = resultItem.ParentLink;
//catalogContentBase.StartPublish = resultItem.StartPublish;
//catalogContentBase.StopPublish = resultItem.StopPublish;
//catalogContentBase.ContentTypeID = resultItem.ContentTypeID;
//catalogContentBase.ApplicationId = resultItem.ApplicationId;

////if (!String.IsNullOrEmpty(resultItem.LanguageName))
////{
////    catalogContentBase.Language = CultureInfo.GetCultureInfo(resultItem.LanguageName);
////}

////if (resultItem.StartPublishedNormalized != null)
////{
////    catalogContentBase.Status = VersionStatus.Published;
////}

//var entryContentBase = catalogContentBase as EntryContentBase;
//if (entryContentBase != null)
//{
//    entryContentBase.MetaClassId = resultItem.MetaClassId;
//    entryContentBase.Code = resultItem.Code;
//}

//var catalogContent = catalogContentBase as CatalogContent;
//if (catalogContent != null)
//{
//    //catalogContent.DefaultCurrency = resultItem.DefaultCurrency;
//    //catalogContent.WeightBase = resultItem.WeightBase;
//    //catalogContent.LengthBase = resultItem.LengthBase;
//}

//contentList.Add(catalogContentBase);

//if (resultItem.VariationLinks != null && resultItem.VariationLinks.Any())
//{
//    if (typeof(ProductContent).IsAssignableFrom(type))
//    {
//        linkedVariationLinks.AddRange(resultItem.VariationLinks);
//    }
//}