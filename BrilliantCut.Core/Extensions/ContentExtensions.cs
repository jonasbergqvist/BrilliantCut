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

    using Mediachase.Commerce.Inventory;
    using Mediachase.Commerce.Markets;

    using ICategorizable = EPiServer.Commerce.Catalog.ContentTypes.ICategorizable;

    public static class ContentExtensions
    {
        public static IEnumerable<string> CategoryNames(this CatalogContentBase content)
        {
            ICategorizable productContent = content as ICategorizable;
            if (productContent == null)
            {
                return Enumerable.Empty<string>();
            }

            IContentLoader contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            IEnumerable<ContentReference> contentLinks = productContent.GetNodeRelations().Select(x => x.Target);

            IEnumerable<IContent> contentItems = contentLoader.GetItems(
                contentLinks: contentLinks,
                language: content.Language);
            return contentItems.Select(x => x.Name);
        }

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

        public static string DefaultCurrency(this CatalogContentBase content)
        {
            CatalogContent catalogContent = content as CatalogContent;
            if (catalogContent != null)
            {
                return catalogContent.DefaultCurrency;
            }

            return null;
        }

        public static string DefaultImageUrl(this CatalogContentBase content)
        {
            IAssetContainer assetContainer = content as IAssetContainer;
            if (assetContainer == null)
            {
                return string.Empty;
            }

            AssetUrlResolver assetUrlResolver = ServiceLocator.Current.GetInstance<AssetUrlResolver>();
            return assetUrlResolver.GetAssetUrl<IContentImage>(assetContainer: assetContainer);
        }

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
                IPricing[] variants = ServiceLocator.Current.GetInstance<IContentLoader>()
                    .GetItems(contentLinks: variantLinks, language: content.Language).OfType<IPricing>().ToArray();
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
            if (price == null)
            {
                return default(double?);
            }

            return Convert.ToDouble(value: price.UnitPrice.Amount);
        }

        public static IEnumerable<Inventory> Inventories(this CatalogContentBase content)
        {
            VariationContent stockPlacement = content as VariationContent;
            if (stockPlacement == null)
            {
                return Enumerable.Empty<Inventory>();
            }

            InventoryLoader inventoyLoader = ServiceLocator.Current.GetInstance<InventoryLoader>();
            ContentReference contentLink = stockPlacement.InventoryReference;

            return !ContentReference.IsNullOrEmpty(contentLink: contentLink)
                       ? inventoyLoader.GetStockPlacement(contentLink: contentLink)
                       : new ItemCollection<Inventory>();
        }

        public static string LanguageName(this ILocale content)
        {
            return content.Language.Name;
        }

        public static string LengthBase(this CatalogContentBase content)
        {
            CatalogContent catalogContent = content as CatalogContent;
            if (catalogContent != null)
            {
                return catalogContent.LengthBase;
            }

            return null;
        }

        public static string LinkUrl(this CatalogContentBase content)
        {
            return ServiceLocator.Current.GetInstance<UrlResolver>().GetUrl(
                contentLink: content.ContentLink,
                language: content.Language.Name);
        }

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

        public static IEnumerable<ContentReference> NodeLinks(this CatalogContentBase content)
        {
            ICategorizable productContent = content as ICategorizable;
            if (productContent == null)
            {
                return Enumerable.Empty<ContentReference>();
            }

            return productContent.GetNodeRelations().Select(x => x.Target);
        }

        public static IEnumerable<ContentReference> ParentProducts(this CatalogContentBase content)
        {
            VariationContent variationContent = content as VariationContent;
            if (variationContent == null)
            {
                return Enumerable.Empty<ContentReference>();
            }

            return variationContent.GetParentProducts();
        }

        public static IEnumerable<Price> Prices(this CatalogContentBase content)
        {
            IPricing pricing = content as IPricing;
            if (pricing == null)
            {
                return Enumerable.Empty<Price>();
            }

            return pricing.GetPrices() ?? Enumerable.Empty<Price>();
        }

        public static IEnumerable<string> SelectedMarkets(this EntryContentBase content)
        {
            IMarketService marketService = ServiceLocator.Current.GetInstance<IMarketService>();

            return marketService.GetAllMarkets()
                .Where(
                    market => !content.MarketFilter.Contains(
                                  value: market.MarketId.Value,
                                  comparer: StringComparer.OrdinalIgnoreCase)).Select(market => market.MarketId.Value);
        }

        public static string ThumbnailUrl(this CatalogContentBase content)
        {
            IAssetContainer assetContainer = content as IAssetContainer;
            if (assetContainer == null)
            {
                return null;
            }

            return ServiceLocator.Current.GetInstance<ThumbnailUrlResolver>().GetThumbnailUrl(
                content: assetContainer,
                propertyName: "Thumbnail");
        }

        public static double? TotalInStock(this VariationContent content)
        {
            IWarehouseInventoryService warehouseInventoryService =
                ServiceLocator.Current.GetInstance<IWarehouseInventoryService>();

            IEnumerable<IWarehouseInventory> allWarehouses = warehouseInventoryService.ListAll();
            if (!allWarehouses.Any())
            {
                return default(double?);
            }

            decimal totalInStock = warehouseInventoryService.ListAll()
                .Where(x => x.CatalogKey.CatalogEntryCode == content.Code).Select(x => x.InStockQuantity).Sum();

            return Convert.ToDouble(value: totalInStock);
        }

        public static IEnumerable<ContentReference> Variations(this CatalogContentBase content)
        {
            ProductContent productContent = content as ProductContent;
            if (productContent == null || ContentReference.IsNullOrEmpty(contentLink: productContent.ContentLink))
            {
                return Enumerable.Empty<ContentReference>();
            }

            return productContent.GetVariants();
        }

        public static string WeightBase(this CatalogContentBase content)
        {
            CatalogContent catalogContent = content as CatalogContent;
            if (catalogContent != null)
            {
                return catalogContent.WeightBase;
            }

            return null;
        }
    }
}