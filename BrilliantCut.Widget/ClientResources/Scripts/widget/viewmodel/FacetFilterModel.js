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

        profileName: "brilliantcut",

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
            this._facetFilterStore = this._facetFilterStore || registry.get("brilliantcut");
        },

        populateData: function () {
            // summary:
            //      Loads data.
            // tags:
            //		protected

            var filterModel = this.profile.get(this.profileName);
            if (!filterModel) {
                filterModel = {
                    valueString: "",
                    listingMode: "0",
                }
            }

            var queryParameters = {
                referenceId: this.context.id,
                query: "getchildren",
                filterModel: filterModel.valueString,
                listingMode: filterModel.listingMode,
            };

            return when(this._facetFilterStore.query(queryParameters)).then(lang.hitch(this, function (filters) {
                return filters;
            }));
        },

        getCheckedItems: function (id) {
            var filterModel = this.profile.get(this.profileName);
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

        getListingMode: function() {
            var filterModel = this.profile.get(this.profileName);
            if (!filterModel) {
                return "0";
            }

            return filterModel.listingMode;
        },

        productGrouped: function () {
            var filterModel = this.profile.get(this.profileName);
            if (!filterModel) {
                return false;
            }

            return filterModel.productGrouped;
        },

        getFacetContainerH: function () {
            var filterModel = this.profile.get(this.profileName);
            if (!filterModel) {
                return 0;
            }

            return filterModel.facetContainerH;
        },

        updateList: function (parent, modelFilters, productGrouped, listingMode, searchResultList) {

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

            var filterModel = { value: models, productGrouped: productGrouped, listingMode: listingMode, valueString: modelsString, facetContainerH: this.facetContainerH }; //contentLink: this.context.id, 
            if (modelFilters.length > 0) {
                this.profile.set(this.profileName, filterModel);
            }

            if (listingMode === "1") {
                topic.publish("/epi/shell/context/request", { uri: this.context.uri }, { sender: this, forceContextChange: true, forceReload: true });
            } else if (listingMode === "2") {
                parent.widgetChange();

                var queryOptions = { ignore: ["query"], parentId: this.context.id, sort: [{ attribute: "name" }] };
                var queryParameters = {
                    referenceId: this.context.id,
                    query: "getchildren",
                    filterModel: filterModel.valueString,
                    productGrouped: filterModel.productGrouped,
                    listingMode: filterModel.listingMode
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
