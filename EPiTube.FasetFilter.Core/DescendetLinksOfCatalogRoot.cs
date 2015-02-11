using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find.Cms;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;

namespace EPiTube.FasetFilter.Core
{
    [ServiceConfiguration(typeof(IReindexInformation))]
    public class DescendetLinksOfCatalogRoot : IReindexInformation
    {
        private readonly IContentLoader _contentLoader;
        private readonly ReferenceConverter _referenceConverter;
        private readonly LanguageSelectorFactory _languageSelectorFactory;

        public DescendetLinksOfCatalogRoot(ReferenceConverter referenceConverter,
            IContentLoader contentLoader,
            LanguageSelectorFactory languageSelectorFactory)
        {
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
            _languageSelectorFactory = languageSelectorFactory;
        }

        /// <summary>
        /// Returns all descendents of the <see cref="Root"/>.
        /// </summary>
        public IEnumerable<ReindexTarget> ReindexTargets
        {
            get
            {
                var catalogs = _contentLoader.GetChildren<CatalogContent>(Root, _languageSelectorFactory.AutoDetect(true));
                foreach (var catalogContent in catalogs)
                {
                    var reindexTarget = new ReindexTarget()
                    {
                        ContentLinks = GetGetDescendents(catalogContent.ContentLink)
                    };

                    var languages = catalogContent.ExistingLanguages.ToList();
                    if (!languages.Select(x => x.Name).Contains(catalogContent.DefaultLanguage))
                    {
                        languages.Add(CultureInfo.GetCultureInfo(catalogContent.DefaultLanguage));
                    }

                    reindexTarget.Languages = languages;
                    yield return reindexTarget;
                }
            }
        }

        private IEnumerable<ContentReference> GetGetDescendents(ContentReference contentLink)
        {
            var children = _contentLoader.GetDescendents(contentLink).ToList();
            children.Add(contentLink);

            return children;
        }


        /// <summary>
        /// Gets the reference of the catalog root.
        /// </summary>
        public ContentReference Root
        {
            get { return _referenceConverter.GetRootLink(); }
        }
    }
}
