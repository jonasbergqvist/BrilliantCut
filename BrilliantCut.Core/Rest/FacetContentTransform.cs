using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Cms.Shell.UI.Rest;
using EPiServer.Cms.Shell.UI.Rest.Models;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using BrilliantCut.Core.Models;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Storage;

namespace BrilliantCut.Core.Rest
{
    [ServiceConfiguration(typeof(IModelTransform))]
    public class FacetContentTransform : IModelTransform
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly ContentTypeModelRepository _contentTypeModelRepository;

        public FacetContentTransform(IContentTypeRepository contentTypeRepository, ContentTypeModelRepository contentTypeModelRepository)
        {
            _contentTypeRepository = contentTypeRepository;
            _contentTypeModelRepository = contentTypeModelRepository;
        }

        public bool CanExecute(Type targetType, DefaultQueryParameters queryParameters)
        {
            return (typeof(StructureStoreContentDataModel)).IsAssignableFrom(targetType);
        }

        public TransformOrder Order
        {
            get { return TransformOrder.Transform + 356; }
        }

        public IEnumerable<IModelTransformContext> Execute(IEnumerable<IModelTransformContext> models)
        {
            var modelList = models.ToList();

            foreach (var model in modelList)
            {
                var facetContent = model.Source as IFacetContent;
                if (facetContent != null)
                {
                    var properties = model.Target.Properties;

                    properties["MetaClassName"] = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, facetContent.MetaClassId).FriendlyName;
                    properties["StartPublish"] = facetContent.StartPublish;
                    properties["StopPublish"] = facetContent.StopPublish;

                    var contentType = _contentTypeRepository.Load(facetContent.ContentTypeID);
                    var contentTypeModel = _contentTypeModelRepository.List()
                        .FirstOrDefault(x => x.ExistingContentType == contentType);
                    if (contentTypeModel == null)
                    {
                        continue;
                    }

                    if (typeof (EntryContentBase).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["Code"] = facetContent.Code;
                    }

                    if (typeof(VariationContent).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["Price"] = facetContent.DefaultPrice.ToString();

                        var instockQuantity = facetContent.Inventories.Sum(x => x.InStockQuantity);
                        var reorderMinQuantity = facetContent.Inventories.Max(x => x.ReorderMinQuantity);

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

                    if (typeof(NodeContent).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["Code"] = facetContent.Code;
                    }

                    if (typeof (CatalogContent).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["WeightBase"] = facetContent.WeightBase;
                        properties["DefaultCurrency"] = facetContent.DefaultCurrency;
                        properties["LengthBase"] = facetContent.LengthBase;
                    }

                    if (typeof(IAssetContainer).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["Thumbnail"] = facetContent.ThumbnailPath;
                    }

                    model.Target.TypeIdentifier = contentTypeModel.ModelType.FullName.ToLowerInvariant();

                    SetCurrentCategoryRelation(model, properties, facetContent);
                    SetHasChildrenForEntryContent(model, facetContent);

                    //        var catalogProperties = new Dictionary<string, Func<object, object>>
                    //    {
                    //        {CatalogProperty(c => c.DefaultLanguage), GetTranslatedLanguage},
                    //        {CatalogProperty(c => c.Owner), getDefaultValue}
                    //    };
                }
            }

            return modelList;
        }

        private static void SetHasChildrenForEntryContent(IModelTransformContext context, IFacetContent source)
        {
            var structureStoreContentDataModel = context.Target as StructureStoreContentDataModel;
            if (structureStoreContentDataModel == null)
            {
                return;
            }

            structureStoreContentDataModel.HasChildren = source.VariationLinks != null && source.VariationLinks.Any();
        }

        private static void SetCurrentCategoryRelation(IModelTransformContext context, PropertyDictionary properties, IFacetContent source)
        {

            ContentReference currentCategoryLink;
            if (ContentReference.TryParse(context.QueryParameters.AllParameters["currentCategory"], out currentCategoryLink))
            {
                properties["IsRelatedToCurrentCategory"] =
                     source.NodeLinks != null && 
                     source.NodeLinks.Any(link => link.CompareToIgnoreWorkID(currentCategoryLink));
            }
            else
            {
                properties["IsRelatedToCurrentCategory"] = true;
            }
        }
    }
}
