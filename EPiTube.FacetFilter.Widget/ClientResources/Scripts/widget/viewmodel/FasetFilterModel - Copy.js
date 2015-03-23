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
    "epi/shell/_StatefulGetterSetterMixin"//,
    //"epi-cms/Profile"
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
    _StatefulGetterSetterMixin//,
    //Profile
) {
    return declare([Stateful, _StatefulGetterSetterMixin], {

        //_filterContentStore: null,

        //_fasetFilterStore: null,

        //context: null,

        ////profile: null,

        ////initialize: function () {
        ////    // summary:
        ////    //		Initialize module
        ////    //
        ////    // description:

        ////    this.inherited(arguments);

        ////    //this.profile = dependency.resolve("epi.shell.Profile");

        ////    //var def = profile.getContentLanguage();
        ////    //when(def, function (language) {
        ////    //    //Set the current edit language on the profile object (this property has the same life-cycle as the application)
        ////    //    //This property is used by the xhrwrapper
        ////    //    ApplicationSettings.currentContentLanguage = profile.contentLanguage = language;
        ////    //});

        ////    //return def;
        ////},

        //postscript: function () {
        //    // summary:
        //    //      Post properties mixin handler.
        //    // description:
        //    //		Set up model and resource for template binding.
        //    // tags:
        //    //		protected

        //    this.inherited(arguments);

        //    var registry = dependency.resolve("epi.storeregistry");
        //    //this._filterContentStore = this._filterContentStore || registry.get("epi.commerce.filtercontent");
        //    this._fasetFilterStore = this._fasetFilterStore || registry.get("epi.commerce.fasetfilter");
        //},

        //populateData: function (context) {
        //    // summary:
        //    //      Loads data.
        //    // tags:
        //    //		protected
        //    this.context = context;

        //    return when(this._fasetFilterStore.refresh(context.id), lang.hitch(this, function (filters) {
        //        // TODO: Order the filters by filters.filterContent.name
        //        return filters;
        //    }));
        //},

        //getCheckedItems: function (id) {
        //    //when(this._profile.get("epitubefilter"), lang.hitch(this, function (filterModel) {
        //    //    var models = [];
        //    //    if (!filterModel) {
        //    //        return models;
        //    //    }

        //    //    filterModel.forEach(function (filter) {
        //    //        filter.forEach(function (filterValue) {
        //    //            if (filterValue.name === id) {
        //    //                models.push(filterValue.value);
        //    //            }
        //    //        });
        //    //    });

        //    //    return models;
        //    //}));

        //    //return when(this._filterContentStore.refresh(id), lang.hitch(this, function (checked) {
        //    //    return checked;
        //    //}));
        //},

        //updateList: function (modelFilters) {
        //    var models = [];
        //    modelFilters.forEach(function (modelFilter) {
        //        var options = [];
        //        modelFilter.checkboxes.forEach(function(checkbox) {
        //            if (checkbox.checked === true) {
        //                options.push({ name: checkbox.name, value: checkbox.name });
        //            }
        //        });

        //        models.push({ name: modelFilter.filter.filterContent.name, value: options });
        //    });

        //    var filterModel = { contentLink: this.context.id, value: models };
        //    //this.profile.set("epitubefilter", filterModel);

        //    //this._filterContentStore.put(filterModel);

        //    topic.publish("/epi/shell/context/request", { uri: this.context.uri }, { sender: this, forceContextChange: true, forceReload: true });
        //},

        //_setValueAttr: function (value) {
        //    // summary:
        //    //      Sets value for this widget.

        //    this._set("value", value);
        //}
    });
});
