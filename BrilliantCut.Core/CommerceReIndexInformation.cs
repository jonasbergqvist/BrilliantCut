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

namespace BrilliantCut.Core
{
    /// <summary>
    /// Allows find to index catalog content
    /// </summary>
    [ServiceConfiguration(typeof(IReindexInformation), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CommerceReIndexInformation : IReindexInformation
    {
        private readonly IContentLoader _contentLoader;
        private readonly LanguageSelectorFactory _langugSelectorFactory;
        private readonly ReferenceConverter _referenceConverter;

        private bool? _allowIndexingCatalogContent; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CommerceReIndexInformation"/> class.
        /// </summary>
        /// <param name="referenceConverter">The reference converter.</param>
        /// <param name="contentLoader">The content loader.</param>
        /// <param name="langugSelectorFactory">The language selector factory.</param>
        public CommerceReIndexInformation(ReferenceConverter referenceConverter, IContentLoader contentLoader, LanguageSelectorFactory langugSelectorFactory)
        {
            _contentLoader = contentLoader;
            _langugSelectorFactory = langugSelectorFactory;
            _referenceConverter = referenceConverter;
        }

        /// <summary>
        /// Determine if the class should be used to index catalog content.
        /// </summary>
        public virtual bool AllowIndexingCatalogContent
        {
            get
            {
                if (!_allowIndexingCatalogContent.HasValue)
                {
                    bool allowIndexingCatalogContent;
                    _allowIndexingCatalogContent = !bool.TryParse(ConfigurationManager.AppSettings["episerver:FindIndexCatalogContent"], out allowIndexingCatalogContent) || allowIndexingCatalogContent;
                }

                return _allowIndexingCatalogContent.Value;
            }
        }

        /// <summary>
        /// Returns all descendants of the <see cref="Root"/>.
        /// </summary>
        public virtual IEnumerable<ReindexTarget> ReindexTargets
        {
            get
            {
                if (!AllowIndexingCatalogContent)
                {
                    yield break;
                }

                var catalogs = GetCatalogs();
                foreach (var catalogContent in catalogs)
                {
                    var reindexTarget = new ReindexTarget
                    {
                        ContentLinks = GetContentToIndex(catalogContent),
                        Languages = GetLanguagesToIndex(catalogContent)
                    };

                    yield return reindexTarget;
                }
            }
        }

        /// <summary>
        /// Gets the catalogs, which will be indexed.
        /// </summary>
        /// <returns>The catalogs that will be indexed.</returns>
        protected virtual IEnumerable<CatalogContent> GetCatalogs()
        {
            return _contentLoader.GetChildren<CatalogContent>(Root, _langugSelectorFactory.AutoDetect(true));
        }

        /// <summary>
        /// Gets content to index for a catalog
        /// </summary>
        /// <param name="catalogContent">The catalog</param>
        /// <returns>The content that will be indexed for a catalog</returns>
        protected virtual IEnumerable<ContentReference> GetContentToIndex(CatalogContent catalogContent)
        {
            return _contentLoader.GetDescendents(catalogContent.ContentLink).Union(new List<ContentReference> { catalogContent.ContentLink });
        }

        /// <summary>
        /// The languages the <paramref name="catalogContent"/> should be indexed in.
        /// </summary>
        /// <param name="catalogContent">The content that will be indexed.</param>
        /// <returns>The languages the <paramref name="catalogContent"/> will be indexed in.</returns>
        protected virtual IEnumerable<CultureInfo> GetLanguagesToIndex(CatalogContent catalogContent)
        {
            var languages = catalogContent.ExistingLanguages.ToList();
            if (!languages.Select(x => x.Name).Contains(catalogContent.DefaultLanguage))
            {
                languages.Add(CultureInfo.GetCultureInfo(catalogContent.DefaultLanguage));
            }

            return languages;
        }

        /// <summary>
        /// Gets the reference of the catalog root.
        /// </summary>
        public virtual ContentReference Root
        {
            get { return _referenceConverter.GetRootLink(); }
        }
    }
}

