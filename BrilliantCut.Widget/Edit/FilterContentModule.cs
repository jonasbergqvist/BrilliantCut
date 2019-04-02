// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterContentModule.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Widget.Edit
{
    using System.Collections.Generic;

    using EPiServer.Framework.Web.Resources;
    using EPiServer.Shell.Modules;

    public class FilterContentModule : ShellModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterContentModule"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="routeBasePath">The route base path.</param>
        /// <param name="resourceBasePath">The resource base path.</param>
        public FilterContentModule(string name, string routeBasePath, string resourceBasePath)
            : base(name: name, routeBasePath: routeBasePath, resourceBasePath: resourceBasePath)
        {
        }

        /// <summary>
        /// Creates the view module to be rendered as setting of CommerceApplication (in client side).
        /// </summary>
        /// <param name="moduleTable">The module table.</param>
        /// <param name="clientResourceService">The client resource service.</param>
        /// <returns>An object that will be serialized and sent to the client when initializing a view.</returns>
        public override ModuleViewModel CreateViewModel(
            ModuleTable moduleTable,
            IClientResourceService clientResourceService)
        {
            ModuleViewModel model = base.CreateViewModel(
                moduleTable: moduleTable,
                clientResourceService: clientResourceService);
            this.AddUiRoute(viewModel: model);
            return model;
        }

        private void AddUiRoute(ModuleViewModel viewModel)
        {
            viewModel.Routes.Add(
                new ModuleRoutePair(
                    routeBasePath: this.ResourceBasePath,
                    routeDefaults: new Dictionary<string, string> { { "moduleArea", "BrilliantCut" } }));
        }
    }
}