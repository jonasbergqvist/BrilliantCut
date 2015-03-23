using System;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Find;
using EPiServer.Shell.Services.Rest;
using EPiTube.FacetFilter.Core.Extensions;
using EPiTube.FacetFilter.Core.Models;

namespace EPiTube.FacetFilter.Core.Service
{
    public class SearchSortingService
    {
        public ISearch Sort(SortColumn sortColumn, ISearch query)
        {
            if (String.IsNullOrEmpty(sortColumn.ColumnName))
            {
                return query;
            }

            var catalogContentSearch = query as ITypeSearch<CatalogContentBase>;
            if (catalogContentSearch != null)
            {
                return GetSortedSearch(sortColumn, catalogContentSearch);
            }

            var otherSupportedModel = query as ITypeSearch<IFacetContent>;
            if (otherSupportedModel != null)
            {
                return GetSortedSearch(sortColumn, otherSupportedModel);
            }

            return query;
        }

        private static ITypeSearch<object> GetSortedSearch(SortColumn sortColumn, ITypeSearch<CatalogContentBase> query)
        {
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

        private static ITypeSearch<object> GetSortedSearch(SortColumn sortColumn, ITypeSearch<IFacetContent> query)
        {
            switch (sortColumn.ColumnName)
            {
                case "name":
                    {
                        return sortColumn.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                    }
                case "code":
                    {
                        return sortColumn.SortDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code);
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
                        return sortColumn.SortDescending ? query.OrderByDescending(x => x.MetaClassId) : query.OrderBy(x => x.MetaClassId);
                    }
                default:
                    {
                        return sortColumn.SortDescending ? query.OrderByDescending(x => x.ContentTypeID) : query.OrderBy(x => x.ContentTypeID);
                    }
            }
        }
    }
}
