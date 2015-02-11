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
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Storage;

namespace EPiTube.FasetFilter.Core.Rest
{
    [ServiceConfiguration(typeof(IModelTransform))]
    public class EPiTubeModelTransform : IModelTransform
    {
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly ContentTypeModelRepository _contentTypeModelRepository;

        public EPiTubeModelTransform(IContentTypeRepository contentTypeRepository, ContentTypeModelRepository contentTypeModelRepository)
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
                var epiTubeModel = model.Source as EPiTubeModel;
                if (epiTubeModel != null)
                {
                    var properties = model.Target.Properties;

                    properties["MetaClassName"] = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, epiTubeModel.MetaClassId).FriendlyName;
                    properties["StartPublish"] = epiTubeModel.StartPublish;
                    properties["StopPublish"] = epiTubeModel.StopPublish;

                    var contentType = _contentTypeRepository.Load(epiTubeModel.ContentTypeId);
                    var contentTypeModel = _contentTypeModelRepository.List()
                        .FirstOrDefault(x => x.ExistingContentType == contentType);
                    if (contentTypeModel == null)
                    {
                        continue;
                    }

                    if (typeof (EntryContentBase).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["Code"] = epiTubeModel.Code;
                    }

                    if (typeof(VariationContent).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["Price"] = epiTubeModel.DefaultPrice.ToString();

                        var instockQuantity = epiTubeModel.Inventories.Sum(x => x.InStockQuantity);
                        var reorderMinQuantity = epiTubeModel.Inventories.Max(x => x.ReorderMinQuantity);

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
                        properties["Code"] = epiTubeModel.Code;
                    }

                    if (typeof (CatalogContent).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["WeightBase"] = epiTubeModel.WeightBase;
                        properties["DefaultCurrency"] = epiTubeModel.DefaultCurrency;
                        properties["LengthBase"] = epiTubeModel.LengthBase;
                    }

                    if (typeof(IAssetContainer).IsAssignableFrom(contentTypeModel.ModelType))
                    {
                        properties["Thumbnail"] = epiTubeModel.ThumbnailPath;
                    }

                    model.Target.TypeIdentifier = contentTypeModel.ModelType.FullName.ToLowerInvariant();

                    SetCurrentCategoryRelation(model, properties, epiTubeModel);
                    SetHasChildrenForEntryContent(model, epiTubeModel);

                    //        var catalogProperties = new Dictionary<string, Func<object, object>>
                    //    {
                    //        {CatalogProperty(c => c.DefaultLanguage), GetTranslatedLanguage},
                    //        {CatalogProperty(c => c.Owner), getDefaultValue}
                    //    };
                }
            }

            return modelList;
        }

        private static void SetHasChildrenForEntryContent(IModelTransformContext context, EPiTubeModel source)
        {
            var structureStoreContentDataModel = context.Target as StructureStoreContentDataModel;
            if (structureStoreContentDataModel == null)
            {
                return;
            }

            structureStoreContentDataModel.HasChildren = source.HasChildren;
        }

        private static void SetCurrentCategoryRelation(IModelTransformContext context, PropertyDictionary properties, EPiTubeModel source)
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
