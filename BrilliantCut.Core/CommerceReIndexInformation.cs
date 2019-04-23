// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommerceReIndexInformation.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;

    using EPiServer;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.Find.Cms;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Catalog;

    /// <summary>
    /// Allows find to index catalog content
    /// Implements the <see cref="EPiServer.Find.Cms.IReindexInformation" />
    /// </summary>
    /// <seealso cref="EPiServer.Find.Cms.IReindexInformation" />
    [ServiceConfiguration(typeof(IReindexInformation), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CommerceReIndexInformation : IReindexInformation
    {
        /// <summary>
        /// The content loader
        /// </summary>
        private readonly IContentLoader contentLoader;

        /// <summary>
        /// The language selector factory
        /// </summary>
        private readonly LanguageSelectorFactory languageSelectorFactory;

        /// <summary>
        /// The reference converter
        /// </summary>
        private readonly ReferenceConverter referenceConverter;

        /// <summary>
        /// The allow indexing catalog content
        /// </summary>
        private bool? allowIndexingCatalogContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommerceReIndexInformation" /> class.
        /// </summary>
        /// <param name="referenceConverter">The reference converter.</param>
        /// <param name="contentLoader">The content loader.</param>
        /// <param name="languageSelectorFactory">The language selector factory.</param>
        public CommerceReIndexInformation(
            ReferenceConverter referenceConverter,
            IContentLoader contentLoader,
            LanguageSelectorFactory languageSelectorFactory)
        {
            this.contentLoader = contentLoader;
            this.languageSelectorFactory = languageSelectorFactory;
            this.referenceConverter = referenceConverter;
        }

        /// <summary>
        /// Gets a value indicating whether [the class should be used to index catalog content].
        /// </summary>
        /// <value><c>true</c> if [the class should be used to index catalog content]; otherwise, <c>false</c>.</value>
        public virtual bool AllowIndexingCatalogContent
        {
            get
            {
                if (this.allowIndexingCatalogContent.HasValue)
                {
                    return this.allowIndexingCatalogContent.Value;
                }

                try
                {
                    bool allowIndexingCatalogContentSetting;
                    this.allowIndexingCatalogContent = !bool.TryParse(
                                                           ConfigurationManager.AppSettings["episerver:FindIndexCatalogContent"],
                                                           result: out allowIndexingCatalogContentSetting)
                                                       || allowIndexingCatalogContentSetting;
                }
                catch (NotSupportedException)
                {
                    return false;
                }

                return this.allowIndexingCatalogContent.Value;
            }
        }

        /// <summary>
        /// Gets all descendants of the <see cref="Root" />.
        /// </summary>
        /// <value>The reindex targets.</value>
        public virtual IEnumerable<ReindexTarget> ReindexTargets
        {
            get
            {
                if (!this.AllowIndexingCatalogContent)
                {
                    yield break;
                }

                IEnumerable<CatalogContent> catalogs = this.GetCatalogs();
                foreach (CatalogContent catalogContent in catalogs)
                {
                    ReindexTarget reindexTarget = new ReindexTarget
                                                      {
                                                          ContentLinks =
                                                              this.GetContentToIndex(catalogContent: catalogContent),
                                                          Languages = this.GetLanguagesToIndex(
                                                              catalogContent: catalogContent)
                                                      };

                    yield return reindexTarget;
                }
            }
        }

        /// <summary>
        /// Gets the reference of the catalog root.
        /// </summary>
        /// <value>The root.</value>
        public virtual ContentReference Root
        {
            get
            {
                return this.referenceConverter.GetRootLink();
            }
        }

        /// <summary>
        /// Gets the catalogs, which will be indexed.
        /// </summary>
        /// <returns>The catalogs that will be indexed.</returns>
        protected virtual IEnumerable<CatalogContent> GetCatalogs()
        {
            return this.contentLoader.GetChildren<CatalogContent>(
                contentLink: this.Root,
                settings: this.languageSelectorFactory.AutoDetect(true));
        }

        /// <summary>
        /// Gets content to index for a catalog
        /// </summary>
        /// <param name="catalogContent">The catalog</param>
        /// <returns>The content that will be indexed for a catalog</returns>
        protected virtual IEnumerable<ContentReference> GetContentToIndex(CatalogContent catalogContent)
        {
            if (catalogContent == null)
            {
                return new List<ContentReference>();
            }

            return this.contentLoader.GetDescendents(contentLink: catalogContent.ContentLink)
                .Union(new List<ContentReference> { catalogContent.ContentLink });
        }

        /// <summary>
        /// The languages the <paramref name="catalogContent" /> should be indexed in.
        /// </summary>
        /// <param name="catalogContent">The content that will be indexed.</param>
        /// <returns>The languages the <paramref name="catalogContent" /> will be indexed in.</returns>
        protected virtual IEnumerable<CultureInfo> GetLanguagesToIndex(CatalogContent catalogContent)
        {
            if (catalogContent == null)
            {
                return new List<CultureInfo>();
            }

            List<CultureInfo> languages = catalogContent.ExistingLanguages.ToList();

            if (!languages.Select(x => x.Name).Contains(value: catalogContent.DefaultLanguage))
            {
                languages.Add(CultureInfo.GetCultureInfo(name: catalogContent.DefaultLanguage));
            }

            return languages;
        }
    }
}