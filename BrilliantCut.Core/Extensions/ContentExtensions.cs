// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentExtensions.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EPiServer;
    using EPiServer.Commerce.Catalog;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Commerce.SpecializedProperties;
    using EPiServer.Core;
    using EPiServer.ServiceLocation;
    using EPiServer.Web.Routing;

    using Mediachase.Commerce.InventoryService;
    using Mediachase.Commerce.Markets;

    using ICategorizable = EPiServer.Commerce.Catalog.ContentTypes.ICategorizable;

    /// <summary>
    /// Class ContentExtensions.
    /// </summary>
    public static class ContentExtensions
    {
        /// <summary>
        ///     The asset URL resolver
        /// </summary>
        private static AssetUrlResolver assetUrlResolver;

        /// <summary>
        ///     The content loader
        /// </summary>
        private static IContentLoader contentLoader;

        /// <summary>
        ///     The inventory loader
        /// </summary>
        private static InventoryLoader inventoryLoader;

        /// <summary>
        ///     The inventory service
        /// </summary>
        private static IInventoryService inventoryService;

        /// <summary>
        ///     The market service
        /// </summary>
        private static IMarketService marketService;

        /// <summary>
        ///     The thumbnail URL resolver
        /// </summary>
        private static ThumbnailUrlResolver thumbnailUrlResolver;

        /// <summary>
        ///     The URL resolver
        /// </summary>
        private static IUrlResolver urlResolver;

        /// <summary>
        ///     Gets or sets the asset URL resolver.
        /// </summary>
        /// <value>The asset URL resolver instance.</value>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static AssetUrlResolver AssetUrlResolver
        {
            get
            {
                if (assetUrlResolver != null)
                {
                    return assetUrlResolver;
                }

                assetUrlResolver = ServiceLocator.Current.GetInstance<AssetUrlResolver>();

                return assetUrlResolver;
            }

            set
            {
                assetUrlResolver = value;
            }
        }

        /// <summary>
        ///     Gets or sets the content loader.
        /// </summary>
        /// <value>The content loader instance.</value>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static IContentLoader ContentLoader
        {
            get
            {
                if (contentLoader != null)
                {
                    return contentLoader;
                }

                contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();

                return contentLoader;
            }

            set
            {
                contentLoader = value;
            }
        }

        /// <summary>
        ///     Gets or sets the inventory loader.
        /// </summary>
        /// <value>The inventory loader instance.</value>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static InventoryLoader InventoryLoader
        {
            get
            {
                if (inventoryLoader != null)
                {
                    return inventoryLoader;
                }

                inventoryLoader = ServiceLocator.Current.GetInstance<InventoryLoader>();

                return inventoryLoader;
            }

            set
            {
                inventoryLoader = value;
            }
        }

        /// <summary>
        ///     Gets or sets the inventory service.
        /// </summary>
        /// <value>The inventory service instance.</value>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static IInventoryService InventoryService
        {
            get
            {
                if (inventoryService != null)
                {
                    return inventoryService;
                }

                inventoryService = ServiceLocator.Current.GetInstance<IInventoryService>();

                return inventoryService;
            }

            set
            {
                inventoryService = value;
            }
        }

        /// <summary>
        ///     Gets or sets the market service.
        /// </summary>
        /// <value>The market service instance.</value>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static IMarketService MarketService
        {
            get
            {
                if (marketService != null)
                {
                    return marketService;
                }

                marketService = ServiceLocator.Current.GetInstance<IMarketService>();

                return marketService;
            }

            set
            {
                marketService = value;
            }
        }

        /// <summary>
        ///     Gets or sets the thumbnail URL resolver.
        /// </summary>
        /// <value>The thumbnail URL resolver instance.</value>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static ThumbnailUrlResolver ThumbnailUrlResolver
        {
            get
            {
                if (thumbnailUrlResolver != null)
                {
                    return thumbnailUrlResolver;
                }

                thumbnailUrlResolver = ServiceLocator.Current.GetInstance<ThumbnailUrlResolver>();

                return thumbnailUrlResolver;
            }

            set
            {
                thumbnailUrlResolver = value;
            }
        }

        /// <summary>
        ///     Gets or sets the URL resolver.
        /// </summary>
        /// <value>The URL resolver instance.</value>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static IUrlResolver UrlResolver
        {
            get
            {
                if (urlResolver != null)
                {
                    return urlResolver;
                }

                urlResolver = ServiceLocator.Current.GetInstance<IUrlResolver>();

                return urlResolver;
            }

            set
            {
                urlResolver = value;
            }
        }

        /// <summary>
        /// Categories the names.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of category names.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static IEnumerable<string> CategoryNames(this CatalogContentBase content)
        {
            ICategorizable productContent = content as ICategorizable;

            if (productContent == null)
            {
                return Enumerable.Empty<string>();
            }

            IEnumerable<ContentReference> contentLinks = productContent.GetNodeRelations().Select(x => x.Parent);

            IEnumerable<IContent> contentItems = ContentLoader.GetItems(
                contentLinks: contentLinks,
                language: content.Language);

            return contentItems.Select(x => x.Name);
        }

        /// <summary>
        /// Gets the code for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The code for the catalog content.</returns>
        public static string Code(this CatalogContentBase content)
        {
            EntryContentBase entryContentBase = content as EntryContentBase;

            if (entryContentBase != null)
            {
                return entryContentBase.Code;
            }

            NodeContent nodeContent = content as NodeContent;

            if (nodeContent != null)
            {
                return nodeContent.Code;
            }

            return null;
        }

        /// <summary>
        /// Get the default currency for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The default currency for the catalog content.</returns>
        public static string DefaultCurrency(this CatalogContentBase content)
        {
            CatalogContent catalogContent = content as CatalogContent;

            if (catalogContent != null)
            {
                return catalogContent.DefaultCurrency;
            }

            return null;
        }

        /// <summary>
        /// Gets the default image URL for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The default image URL for the catalog content.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static string DefaultImageUrl(this CatalogContentBase content)
        {
            IAssetContainer assetContainer = content as IAssetContainer;

            if (assetContainer == null)
            {
                return string.Empty;
            }

            return AssetUrlResolver.GetAssetUrl<IContentImage>(assetContainer: assetContainer);
        }

        /// <summary>
        /// Gets the default price value for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns> The default price value for the catalog content, or null.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static double? DefaultPriceValue(this CatalogContentBase content)
        {
            IPricing pricing = content as IPricing;

            if (pricing == null)
            {
                ProductContent productContent = content as ProductContent;
                if (productContent == null || ContentReference.IsNullOrEmpty(contentLink: productContent.ContentLink))
                {
                    return default(double?);
                }

                IEnumerable<ContentReference> variantLinks = productContent.GetVariants();
                IPricing[] variants = ContentLoader.GetItems(contentLinks: variantLinks, language: content.Language)
                    .OfType<IPricing>().ToArray();

                if (!variants.Any())
                {
                    return default(double?);
                }

                Price[] defaultPrices = variants.Select(x => x.GetDefaultPrice())
                    .Where(x => x != null && x.UnitPrice.Amount > 0).ToArray();

                if (!defaultPrices.Any())
                {
                    return default(double?);
                }

                decimal minAmount = defaultPrices.Min(x => x.UnitPrice.Amount);

                return Convert.ToDouble(value: minAmount);
            }

            Price price = pricing.GetDefaultPrice();

            return price == null ? default(double?) : Convert.ToDouble(value: price.UnitPrice.Amount);
        }

        /// <summary>
        /// Gets the inventories for specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Inventory"/> for the catalog content.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static IEnumerable<Inventory> Inventories(this CatalogContentBase content)
        {
            VariationContent stockPlacement = content as VariationContent;

            if (stockPlacement == null)
            {
                return Enumerable.Empty<Inventory>();
            }

            ContentReference contentLink = stockPlacement.InventoryReference;

            return !ContentReference.IsNullOrEmpty(contentLink: contentLink)
                       ? InventoryLoader.GetStockPlacement(contentLink: contentLink)
                       : new ItemCollection<Inventory>();
        }

        /// <summary>
        /// Gets the language name for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The language name for the catalog content.</returns>
        public static string LanguageName(this ILocale content)
        {
            return content?.Language.Name;
        }

        /// <summary>
        /// Gets the length base for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The length base for the catalog content.</returns>
        public static string LengthBase(this CatalogContentBase content)
        {
            CatalogContent catalogContent = content as CatalogContent;

            return catalogContent != null ? catalogContent.LengthBase : null;
        }

        /// <summary>
        /// Gets the link URL for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The link URL for the catalog content.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static string LinkUrl(this CatalogContentBase content)
        {
            return content == null ? string.Empty : UrlResolver.GetUrl(contentLink: content.ContentLink, language: content.Language.Name);
        }

        /// <summary>
        /// Gets the meta class identifier for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The meta class identifier for the catalog content, or null.</returns>
        public static int? MetaClassId(this CatalogContentBase content)
        {
            NodeContent nodeContent = content as NodeContent;

            if (nodeContent != null)
            {
                return nodeContent.MetaClassId;
            }

            EntryContentBase entryContent = content as EntryContentBase;

            if (entryContent != null)
            {
                return entryContent.MetaClassId;
            }

            return default(int?);
        }

        /// <summary>
        /// Gets the node links for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentReference"/> off node links for the catalog content.</returns>
        public static IEnumerable<ContentReference> NodeLinks(this CatalogContentBase content)
        {
            ICategorizable productContent = content as ICategorizable;

            if (productContent == null)
            {
                return Enumerable.Empty<ContentReference>();
            }

            return productContent.GetNodeRelations().Select(x => x.Parent);
        }

        /// <summary>
        /// Gets the parent products for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentReference"/> of parent products for the catalog content.</returns>
        public static IEnumerable<ContentReference> ParentProducts(this CatalogContentBase content)
        {
            VariationContent variationContent = content as VariationContent;

            if (variationContent == null)
            {
                return Enumerable.Empty<ContentReference>();
            }

            return variationContent.GetParentProducts();
        }

        /// <summary>
        /// Gets the prices for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Price"/> for the catalog content.</returns>
        public static IEnumerable<Price> Prices(this CatalogContentBase content)
        {
            IPricing pricing = content as IPricing;

            if (pricing == null)
            {
                return Enumerable.Empty<Price>();
            }

            return pricing.GetPrices() ?? Enumerable.Empty<Price>();
        }

        /// <summary>
        /// Gets the selected markets for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of market id's for the catalog content.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static IEnumerable<string> SelectedMarkets(this EntryContentBase content)
        {
            return MarketService.GetAllMarkets()
                .Where(
                    market => !content.MarketFilter.Contains(
                                  value: market.MarketId.Value,
                                  comparer: StringComparer.OrdinalIgnoreCase)).Select(market => market.MarketId.Value);
        }

        /// <summary>
        /// Gets the thumbnail URL for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The thumbnail URL for the catalog content.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        public static string ThumbnailUrl(this CatalogContentBase content)
        {
            IAssetContainer assetContainer = content as IAssetContainer;

            return assetContainer == null ? null : ThumbnailUrlResolver.GetThumbnailUrl(content: assetContainer, propertyName: "Thumbnail");
        }

        /// <summary>
        /// Gets the 'total in stock' amount for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The 'total in stock' amount for the catalog content, or null.</returns>
        /// <exception cref="T:EPiServer.ServiceLocation.ActivationException">if there are errors resolving the service instance.</exception>
        /// <exception cref="T:System.OverflowException">The sum of stock is larger than <see cref="F:System.Decimal.MaxValue" />.</exception>
        public static double? TotalInStock(this VariationContent content)
        {
            IEnumerable<InventoryRecord> allWarehouses = InventoryService.List();

            if (!allWarehouses.Any())
            {
                return default(double?);
            }

            decimal totalInStock = InventoryService.List().Where(x => x.CatalogEntryCode == content.Code)
                .Select(x => x.PurchaseAvailableQuantity).Sum();

            return Convert.ToDouble(value: totalInStock);
        }

        /// <summary>
        /// Get the variations for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentReference"/> of variations for the catalog content.</returns>
        public static IEnumerable<ContentReference> Variations(this CatalogContentBase content)
        {
            ProductContent productContent = content as ProductContent;

            if (productContent == null || ContentReference.IsNullOrEmpty(contentLink: productContent.ContentLink))
            {
                return Enumerable.Empty<ContentReference>();
            }

            return productContent.GetVariants();
        }

        /// <summary>
        /// Gets the weight base for the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The wight base for the catalog content.</returns>
        public static string WeightBase(this CatalogContentBase content)
        {
            CatalogContent catalogContent = content as CatalogContent;

            return catalogContent != null ? catalogContent.WeightBase : null;
        }
    }
}