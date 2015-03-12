define([

// Dojo
    "dojo/_base/declare",
    "dojo/Deferred",
    "dojo/_base/lang",
    "dojo/html",
    "dojo/when",
    "dojo/_base/array",
    "dojox/layout/ScrollPane",
    "dojo/parser",
    "dojo/on",

// Dijit
    "dijit/_TemplatedMixin",
    "dijit/_WidgetBase",
    "dijit/layout/ContentPane",
    "dijit/form/CheckBox",

//CMS 
    "epi-cms/_ContentContextMixin",

//Commerce
    "./viewmodel/FasetFilterModel",

//Resources
    "dojo/text!./templates/FasetFilter.html"
], function (

// Doj
    declare,
    Deferred,
    lang,
    html,
    when,
    array,
    ScrollPane,
    parser,
    on,

// Dijit
    _TemplatedMixin,
    _WidgetBase,
    ContentPane,
    CheckBox,

//CMS
    _ContentContextMixin,

//Commerce
    FasetFilterModel,

//Resources
    template

) {
    return declare([_WidgetBase, _TemplatedMixin, _ContentContextMixin], {

        templateString: template,

        modelClassName: FasetFilterModel,

        model: null,

        modelFilters: null,

        constructor: function() {
            this.modelFilters = [];
        },

        postCreate: function () {
            this.inherited(arguments);

            if (!this.model && this.modelClassName) {
                var modelClass = declare(this.modelClassName);
                this.set("model", new modelClass());
            }


        },

        startup: function () {
            this.inherited(arguments);

            this.fasedEnabledPoint.checked = this.model.isEnabled();
            this.productGroupingPoint.checked = this.model.productGrouped();

            dojo.connect(this.fasedEnabledPoint, "change", lang.hitch(this, function () {

                if (this.fasedEnabledPoint.checked) {
                    this.faset.style.display = "";
                } else {
                    this.faset.style.display = "none";
                }

                this.updateList();
            }));

            dojo.connect(this.productGroupingPoint, "change", lang.hitch(this, function () {
                this.updateList();
            }));

            when(this.getCurrentContext(), lang.hitch(this, this.contextChanged));
        },


        contextChanged: function (context, sender) {

            this.model.populateData(context).then(lang.hitch(this, function(filters) {
                filters.forEach(lang.hitch(this, function (filter) {
                    var checkedItems = this.model.getCheckedItems(filter.filterContent.name);
   
                    var modelFilter = null;
                    this.modelFilters.forEach(lang.hitch(this, function(existingFilter) {
                        if (existingFilter.filter.filterContent.name === filter.filterContent.name) {
                            modelFilter = existingFilter;
                        }
                    }));

                    var hasModelFilter = modelFilter !== null;
                    if (!hasModelFilter) {
                        require([filter.attribute.filterPath], lang.hitch(this, function(filterClass) {
                            modelFilter = new filterClass();

                            modelFilter.setFilter(filter, checkedItems);
                            modelFilter.Write(lang.hitch(this, this.updateList));

                            this.faset.appendChild(modelFilter.domNode);
                            this.own(modelFilter);
                            this.modelFilters.push(modelFilter);
                        }));
                    } else {
                        modelFilter.setFilter(filter, checkedItems);
                        modelFilter.Write(lang.hitch(this, this.updateList));
                    }

                }));

                for (var i = 0; i < this.modelFilters.length; i++) {
                    var filterExist = false;

                    filters.forEach(lang.hitch(this, function(filter) {
                        if (filter.filterContent.name === this.modelFilters[i].filter.filterContent.name) {
                            filterExist = true;
                        } 
                    }));

                    if (!filterExist) {
                        if (!!this.modelFilters[i].RemoveFilter()) {
                            this.faset.removeChild(this.modelFilters[i].domNode);
                            //this.modelFilters[i].destroy();
                            this.modelFilters.splice(i, 1);
                            i--;
                        }
                    }
                }
            }));
        },

        updateList: function () {
            this.model.updateList(this.modelFilters, this.fasedEnabledPoint.checked, this.productGroupingPoint.checked, true);
        }

    });
});