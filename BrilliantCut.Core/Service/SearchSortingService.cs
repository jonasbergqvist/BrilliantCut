﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchSortingService.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Service
{
    using BrilliantCut.Core.Extensions;
    using BrilliantCut.Core.Models;

    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Find;
    using EPiServer.Shell.Services.Rest;

    /// <summary>
    /// Class SearchSortingService.
    /// </summary>
    public class SearchSortingService
    {
        /// <summary>
        /// Sorts the specified sort column.
        /// </summary>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="query">The query.</param>
        /// <returns>The sorted search.</returns>
        public ISearch Sort(SortColumn sortColumn, ISearch query)
        {
            if (sortColumn == null || string.IsNullOrEmpty(value: sortColumn.ColumnName))
            {
                return query;
            }

            ITypeSearch<CatalogContentBase> catalogContentSearch = query as ITypeSearch<CatalogContentBase>;

            if (catalogContentSearch != null)
            {
                return GetSortedSearch(sortColumn: sortColumn, query: catalogContentSearch);
            }

            ITypeSearch<IFacetContent> otherSupportedModel = query as ITypeSearch<IFacetContent>;

            if (otherSupportedModel != null)
            {
                return GetSortedSearch(sortColumn: sortColumn, query: otherSupportedModel);
            }

            return query;
        }

        /// <summary>
        /// Gets the sorted search.
        /// </summary>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="query">The query.</param>
        /// <returns>An <see cref="ITypeSearch{TSource}"/>.</returns>
        private static ITypeSearch<object> GetSortedSearch(SortColumn sortColumn, ITypeSearch<CatalogContentBase> query)
        {
            switch (sortColumn.ColumnName)
            {
                case "name":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.Name)
                                   : query.OrderBy(x => x.Name);
                    }

                case "code":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.Code())
                                   : query.OrderBy(x => x.Code());
                    }

                case "isPendingPublish":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.Status)
                                   : query.OrderBy(x => x.Status);
                    }

                case "startPublish":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.StartPublish)
                                   : query.OrderBy(x => x.StartPublish);
                    }

                case "stopPublish":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.StopPublish)
                                   : query.OrderBy(x => x.StopPublish);
                    }

                case "metaClassName":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.MetaClassId())
                                   : query.OrderBy(x => x.MetaClassId());
                    }

                default:
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.ContentTypeID)
                                   : query.OrderBy(x => x.ContentTypeID);
                    }
            }
        }

        /// <summary>
        /// Gets the sorted search.
        /// </summary>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="query">The query.</param>
        /// <returns>ITypeSearch&lt;System.Object&gt;.</returns>
        private static ITypeSearch<object> GetSortedSearch(SortColumn sortColumn, ITypeSearch<IFacetContent> query)
        {
            switch (sortColumn.ColumnName)
            {
                case "name":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.Name)
                                   : query.OrderBy(x => x.Name);
                    }

                case "code":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.Code)
                                   : query.OrderBy(x => x.Code);
                    }

                case "startPublish":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.StartPublish)
                                   : query.OrderBy(x => x.StartPublish);
                    }

                case "stopPublish":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.StopPublish)
                                   : query.OrderBy(x => x.StopPublish);
                    }

                case "metaClassName":
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.MetaClassId)
                                   : query.OrderBy(x => x.MetaClassId);
                    }

                default:
                    {
                        return sortColumn.SortDescending
                                   ? query.OrderByDescending(x => x.ContentTypeID)
                                   : query.OrderBy(x => x.ContentTypeID);
                    }
            }
        }
    }
}