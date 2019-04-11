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

    /// <summary>
    /// Class FacetContentTransform.
    /// Implements the <see cref="EPiServer.Cms.Shell.UI.Rest.Models.Transforms.IModelTransform" />
    /// </summary>
    /// <seealso cref="EPiServer.Cms.Shell.UI.Rest.Models.Transforms.IModelTransform" />
    [ServiceConfiguration(typeof(IModelTransform))]
    public class FacetContentTransform : IModelTransform
    {
        /// <summary>
        /// The content type model repository
        /// </summary>
        private readonly ContentTypeModelRepository contentTypeModelRepository;

        /// <summary>
        /// The content type repository
        /// </summary>
        private readonly IContentTypeRepository contentTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FacetContentTransform"/> class.
        /// </summary>
        /// <param name="contentTypeRepository">The content type repository.</param>
        /// <param name="contentTypeModelRepository">The content type model repository.</param>
        public FacetContentTransform(
            IContentTypeRepository contentTypeRepository,
            ContentTypeModelRepository contentTypeModelRepository)
        {
            this.contentTypeRepository = contentTypeRepository;
            this.contentTypeModelRepository = contentTypeModelRepository;
        }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public TransformOrder Order
        {
            get
            {
                return TransformOrder.Transform + 356;
            }
        }

        /// <summary>
        /// Determines whether this instance can execute the specified target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <returns><c>true</c> if this instance can execute the specified target type; otherwise, <c>false</c>.</returns>
        public bool CanExecute(Type targetType, DefaultQueryParameters queryParameters)
        {
            return typeof(StructureStoreContentDataModel).IsAssignableFrom(c: targetType);
        }

        /// <summary>
        /// Executes the specified models.
        /// </summary>
        /// <param name="models">The models.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IModelTransformContext"/>.</returns>
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

                    ContentType contentType = this.contentTypeRepository.Load(id: facetContent.ContentTypeID);
                    ContentTypeModel contentTypeModel = this.contentTypeModelRepository.List()
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

        /// <summary>
        /// Sets the current category relation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="source">The source.</param>
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

        /// <summary>
        /// Sets the content of the has children for entry.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="source">The source.</param>
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