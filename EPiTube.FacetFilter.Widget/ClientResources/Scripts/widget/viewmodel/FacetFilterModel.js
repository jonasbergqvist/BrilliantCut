define([
// dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/when",
    "dojo/Stateful",
    "dojo/topic",
    // EPi Framework
    "epi/dependency",
    "epi/datetime",
    "epi/shell/_StatefulGetterSetterMixin",
    "epi-cms/Profile"
], function (
// dojo
    declare,
    lang,
    when,
    Stateful,
    topic,
    // EPi Framework
    dependency,
    epiDate,
    _StatefulGetterSetterMixin,
    Profile
) {
    return declare([Stateful, _StatefulGetterSetterMixin], {

        _filterContentStore: null,

        _facetFilterStore: null,

        context: null,

        profile: null,

        facetContainerH: 0,

        postscript: function () {
            // summary:
            //      Post properties mixin handler.
            // description:
            //		Set up model and resource for template binding.
            // tags:
            //		protected

            this.inherited(arguments);

            this.profile = dependency.resolve("epi.shell.Profile");
            var registry = dependency.resolve("epi.storeregistry");
            this._facetFilterStore = this._facetFilterStore || registry.get("epi.commerce.facetfilter");
        },

        populateData: function () {
            // summary:
            //      Loads data.
            // tags:
            //		protected

            var filterModel = this.profile.get("epitubefilter");

            var queryParameters = {
                referenceId: this.context.id,
                query: "getchildren",
                filterModel: filterModel.valueString,
                mainListEnabled: filterModel.mainListEnabled,
            };

            return when(this._facetFilterStore.query(queryParameters)).then(lang.hitch(this, function (filters) {
                return filters;
            }));
        },

        getCheckedItems: function (id) {
            var filterModel = this.profile.get("epitubefilter");
            var models = [];
            if (!filterModel) {
                return models;
            }

            filterModel.value.forEach(function (filter) {
                if (filter.name === id) {
                    filter.value.forEach(function(filterValue) {
                        models.push(filterValue);
                    });
                    
                }
            });

            return models;
        },

        isEnabled: function() {
            var filterModel = this.profile.get("epitubefilter");
            return filterModel.enabled;
        },

        productGrouped: function () {
            var filterModel = this.profile.get("epitubefilter");
            return filterModel.productGrouped;
        },

        mainListEnabled: function () {
            var filterModel = this.profile.get("epitubefilter");
            return filterModel.mainListEnabled;
        },

        getFacetContainerH: function () {
            var filterModel = this.profile.get("epitubefilter");
            return filterModel.facetContainerH;
        },

        updateList: function (parent, modelFilters, enabled, productGrouped, mainListEnabled, searchResultList) {

            if (!this.context) {
                return;
            }

            var modelsString = "";
            var models = [];

            if (modelFilters) {
                modelFilters.forEach(function(modelFilter) {
                    var options = [];
                    modelFilter.filter.filterOptions.forEach(function(filterOption) {
                        if (modelFilter.IsChecked(modelFilter.GetId(filterOption.id, filterOption.value))) {

                            if (options.length === 0) {
                                modelsString += modelFilter.filter.name + "__";
                            }

                            var value = modelFilter.GetValue(filterOption.id, filterOption.value);
                            options.push({ name: filterOption.id, value: value });
                            modelsString += value + "..";
                        }
                    });

                    if (options.length > 0) {
                        modelsString += "__";
                    }

                    models.push({ name: modelFilter.filter.name, value: options });
                });
            }

            var filterModel = { value: models, enabled: enabled, productGrouped: productGrouped, mainListEnabled: mainListEnabled, valueString: modelsString, facetContainerH: this.facetContainerH }; //contentLink: this.context.id, 
            if (modelFilters.length > 0) {
                this.profile.set("epitubefilter", filterModel);
            }

            if (mainListEnabled) {
                topic.publish("/epi/shell/context/request", { uri: this.context.uri }, { sender: this, forceContextChange: true, forceReload: true });
            } else {
                parent.widgetChange();

                var queryOptions = { ignore: ["query"], parentId: this.context.id, sort: [{ attribute: "name" }] };
                var queryParameters = {
                    referenceId: this.context.id,
                    query: "getchildren",
                    filterModel: filterModel.valueString,
                    filterEnabled: filterModel.enabled,
                    productGrouped: filterModel.productGrouped,
                    mainListEnabled: filterModel.mainListEnabled
                };

                searchResultList.set("query", queryParameters, queryOptions);
            }
        },

        _setValueAttr: function (value) {
            // summary:
            //      Sets value for this widget.

            this._set("value", value);
        }
    });
});
