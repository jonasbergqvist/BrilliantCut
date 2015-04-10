define([
    "dojo/_base/declare",
    "epi/_Module",
    "epi/routes"
], function (
    declare,
    _Module,
    routes
) {

    return declare([_Module], {
        // summary: this is the initializer of CommerceUIModule.

        _settings: null,

        constructor: function (settings) {
            this._settings = settings;
        },

        initialize: function () {
            // summary:
            //		Initialize module
            //
            // description:
            //

            this.inherited(arguments);

            // Initialize stores
            this._initializeStores();
        },

        _initializeStores: function () {
            var registry = this.resolveDependency("epi.storeregistry");

            registry.create("brilliantcut", this._getRestPath("facetfilter"), {});
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "BrilliantCut.Widget", storeName: name });
        }
    });
});
