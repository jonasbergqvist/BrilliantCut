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

        _fasetFilterStore: null,

        context: null,

        profile: null,

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
            this._fasetFilterStore = this._fasetFilterStore || registry.get("epi.commerce.fasetfilter");
        },

        populateData: function (context) {
            // summary:
            //      Loads data.
            // tags:
            //		protected
            this.context = context;

            var filterModel = this.profile.get("epitubefilter");
            var queryOptions = { ignore: ["query"], parentId: context.id, sort: [{ attribute: "name" }] };

            var queryParameters = {
                referenceId: context.id,
                query: "getchildren",
                includeProperties: true,
                allLanguages: true,
                keepversion: true,
                filterModel: filterModel.valueString,
                filterEnabled: filterModel.enabled,
                productGrouped: filterModel.productGrouped
            };

            //this.grid.set("query", queryParameters, queryOptions);

            var def = when(this._fasetFilterStore.query(queryParameters)).then(lang.hitch(this, function (filters) {
                return filters;
            }));

            //def.resolve();
            return def;

            //return when(this._fasetFilterStore.refresh(context.id), lang.hitch(this, function (filters) {
            //    // TODO: Order the filters by filters.filterContent.name
            //    return filters;
            //}));
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
                        models.push(filterValue.value);
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

        updateList: function (modelFilters, enabled, productGrouped, refresh) {
            var modelsString = "";
            var models = [];

            if (modelFilters) {
                modelFilters.forEach(function(modelFilter) {
                    var options = [];
                    modelFilter.filter.filterOptions.forEach(function(filterOption) {
                        if (modelFilter.IsChecked(modelFilter.GetId(filterOption.key, filterOption.value))) {

                            if (options.length === 0) {
                                modelsString += modelFilter.filter.filterContent.name + "==";
                            }

                            var value = modelFilter.GetValue(filterOption.key, filterOption.value);
                            options.push({ name: filterOption.key, value: value });
                            modelsString += value + ",,";
                        }
                    });

                    if (options.length > 0) {
                        modelsString += "==";
                    }

                    models.push({ name: modelFilter.filter.filterContent.name, value: options });
                });
            }

            var filterModel = { value: models, enabled: enabled, productGrouped: productGrouped, valueString: modelsString }; //contentLink: this.context.id, 
            this.profile.set("epitubefilter", filterModel);

            if (refresh) {
                topic.publish("/epi/shell/context/request", { uri: this.context.uri }, { sender: this, forceContextChange: true, forceReload: true });
            }
        },

        _setValueAttr: function (value) {
            // summary:
            //      Sets value for this widget.

            this._set("value", value);
        }
    });
});
