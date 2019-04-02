// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetContentTransform.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BrilliantCut.Core.Models;

    using EPiServer.Cms.Shell.UI.Rest;
    using EPiServer.Cms.Shell.UI.Rest.Models;
    using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
    using EPiServer.Commerce.Catalog.ContentTypes;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAbstraction.RuntimeModel;
    using EPiServer.ServiceLocation;

    using Mediachase.Commerce.Catalog;
    using Mediachase.MetaDataPlus.Configurator;

    [ServiceConfiguration(typeof(IModelTransform))]
    public class FacetContentTransform : IModelTransform
    {
        private readonly ContentTypeModelRepository _contentTypeModelRepository;

        private readonly IContentTypeRepository _contentTypeRepository;

        public FacetContentTransform(
            IContentTypeRepository contentTypeRepository,
            ContentTypeModelRepository contentTypeModelRepository)
        {
            this._contentTypeRepository = contentTypeRepository;
            this._contentTypeModelRepository = contentTypeModelRepository;
        }

        public TransformOrder Order
        {
            get
            {
                return TransformOrder.Transform + 356;
            }
        }

        public bool CanExecute(Type targetType, DefaultQueryParameters queryParameters)
        {
            return (typeof(StructureStoreContentDataModel)).IsAssignableFrom(c: targetType);
        }

        public IEnumerable<IModelTransformContext> Execute(IEnumerable<IModelTransformContext> models)
        {
            List<IModelTransformContext> modelList = models.ToList();

            foreach (IModelTransformContext model in modelList)
            {
                IFacetContent facetContent = model.Source as IFacetContent;
                if (facetContent != null)
                {
                    PropertyDictionary properties = model.Target.Properties;

                    if (facetContent.MetaClassId.HasValue)
                    {
                        properties["MetaClassName"] = MetaClass.Load(
                            context: CatalogContext.MetaDataContext,
                            id: facetContent.MetaClassId.Value).FriendlyName;
                    }

                    properties["StartPublish"] = facetContent.StartPublish;
                    properties["StopPublish"] = facetContent.StopPublish;

                    ContentType contentType = this._contentTypeRepository.Load(id: facetContent.ContentTypeID);
                    ContentTypeModel contentTypeModel = this._contentTypeModelRepository.List()
                        .FirstOrDefault(x => x.ExistingContentType == contentType);
                    if (contentTypeModel == null)
                    {
                        continue;
                    }

                    if (typeof(EntryContentBase).IsAssignableFrom(c: contentTypeModel.ModelType))
                    {
                        properties["Code"] = facetContent.Code;
                    }

                    if (typeof(VariationContent).IsAssignableFrom(c: contentTypeModel.ModelType))
                    {
                        properties["Price"] = facetContent.DefaultPriceValue.ToString();

                        decimal instockQuantity = facetContent.Inventories != null
                                                      ? facetContent.Inventories.Sum(x => x.InStockQuantity)
                                                      : 0;
                        decimal reorderMinQuantity = facetContent.Inventories != null
                                                         ? facetContent.Inventories.Max(x => x.ReorderMinQuantity)
                                                         : 0;

                        string status;
                        if (instockQuantity == 0)
                        {
                            status = "unavailable";
                        }
                        else if (instockQuantity < reorderMinQuantity)
                        {
                            status = "low";
                        }
                        else
                        {
                            status = "available";
                        }

                        properties["InStockStatus"] = status;
                        properties["InStockQuantity"] = instockQuantity.ToString();
                    }

                    if (typeof(NodeContent).IsAssignableFrom(c: contentTypeModel.ModelType))
                    {
                        properties["Code"] = facetContent.Code;
                    }

                    if (typeof(CatalogContent).IsAssignableFrom(c: contentTypeModel.ModelType))
                    {
                        properties["WeightBase"] = facetContent.WeightBase;
                        properties["DefaultCurrency"] = facetContent.DefaultCurrency;
                        properties["LengthBase"] = facetContent.LengthBase;
                    }

                    if (typeof(IAssetContainer).IsAssignableFrom(c: contentTypeModel.ModelType))
                    {
                        properties["Thumbnail"] = facetContent.ThumbnailPath;
                    }

                    model.Target.TypeIdentifier = contentTypeModel.ModelType.FullName.ToLowerInvariant();

                    SetCurrentCategoryRelation(context: model, properties: properties, source: facetContent);
                    SetHasChildrenForEntryContent(context: model, source: facetContent);

                    // var catalogProperties = new Dictionary<string, Func<object, object>>
                    // {
                    // {CatalogProperty(c => c.DefaultLanguage), GetTranslatedLanguage},
                    // {CatalogProperty(c => c.Owner), getDefaultValue}
                    // };
                }
            }

            return modelList;
        }

        private static void SetCurrentCategoryRelation(
            IModelTransformContext context,
            PropertyDictionary properties,
            IFacetContent source)
        {
            ContentReference currentCategoryLink;
            if (ContentReference.TryParse(
                context.QueryParameters.AllParameters["currentCategory"],
                result: out currentCategoryLink))
            {
                properties["IsRelatedToCurrentCategory"] = source.NodeLinks != null
                                                           && source.NodeLinks.Any(
                                                               link => link.CompareToIgnoreWorkID(
                                                                   contentReference: currentCategoryLink));
            }
            else
            {
                properties["IsRelatedToCurrentCategory"] = true;
            }
        }

        private static void SetHasChildrenForEntryContent(IModelTransformContext context, IFacetContent source)
        {
            StructureStoreContentDataModel structureStoreContentDataModel =
                context.Target as StructureStoreContentDataModel;
            if (structureStoreContentDataModel == null)
            {
                return;
            }

            structureStoreContentDataModel.HasChildren = source.VariationLinks != null && source.VariationLinks.Any();
        }
    }
}