define([

// Dojo
    "dojo/_base/declare",
    "dojo/Deferred",
    "dojo/aspect",
    "dojo/_base/lang",
    "dojo/dom-geometry",
    "dojo/dom-style",
    "dojo/html",
    "dojo/when",
    "dojo/_base/array",
    "dojox/layout/ScrollPane",
    "dojo/parser",
    "dojo/on",

// Dijit
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/_WidgetBase",
    "dijit/layout/ContentPane",
    "dijit/form/CheckBox",
    "dijit/layout/BorderContainer",
    "dijit/layout/_LayoutWidget",

//CMS 
    "epi-cms/_ContentContextMixin",
    "epi-cms/widget/ContentList",

//Commerce
    "./viewmodel/facetFilterModel",

//Resources
    "dojo/text!./templates/facetFilter.html"
], function (

// Doj
    declare,
    Deferred,
    aspect,
    lang,
    geometry,
    domStyle,
    html,
    when,
    array,
    ScrollPane,
    parser,
    on,

// Dijit
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    _WidgetBase,
    ContentPane,
    CheckBox,
    BorderContainer,
    _LayoutWidget,

//CMS
    _ContentContextMixin,
    ContentList,

//Commerce
    facetFilterModel,

//Resources
    template
) {
    return declare([_LayoutWidget, _TemplatedMixin, _ContentContextMixin, _WidgetsInTemplateMixin], {

        templateString: template,

        modelClassName: facetFilterModel,

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

            this.fasedEnabledPoint.set("checked", this.model.isEnabled());
            this.productGroupingPoint.set("checked", this.model.productGrouped());

            var mainListEnabled = this.model.mainListEnabled();
            this.listMainArea.set("checked", mainListEnabled);
            this.listWidgetArea.set("checked", !mainListEnabled);
        },

        layout: function () {

            var facetContainerh = this.model.getFacetContainerH();
            if (this._contentBox) {
                if (this.listWidgetArea.checked && facetContainerh > 0) {
                    this.listContainer.resize({
                        h: facetContainerh,
                        w: this._contentBox.w,
                        l: 0,
                        t: 0,
                    });
                } else {
                    this.listContainer.resize({
                        h: 50,
                        w: this._contentBox.w,
                        l: 0,
                        t: 0,
                    });
                }

                var facetHeadingSize = geometry.getMarginBox(this.facetHeadingPoint),
                    containerSize = {
                        h: this._contentBox.h - facetHeadingSize.h,
                        w: this._contentBox.w,
                        l: this._contentBox.l,
                        t: this._contentBox.t
                    };

                this.container.resize(containerSize);
            }

            this.listContainer.layout();

            aspect.after(this.listContainer, "resize", lang.hitch(this, function () {
                this.model.facetContainerH = this.listContainer.h;
            }, true));
        },

        startup: function () {
            this.inherited(arguments);

            when(this.getCurrentContext(), lang.hitch(this, this.contextChanged));

            aspect.after(this.fasedEnabledPoint, "onChange", lang.hitch(this, function () {
                this.fasedEnabledPointOnChanged();
            }, true));
            aspect.after(this.productGroupingPoint, "onChange", lang.hitch(this, function () {
                this.updateList();
            }, true));
            aspect.after(this.listMainArea, "onChange", lang.hitch(this, function () {
                this.layout();

                this.updateList();
            }, true));
        },

        fasedEnabledPointOnChanged: function() {
            if (this.fasedEnabledPoint.checked) {
                this.container.style.display = "";
            } else {
                this.container.style.display = "none";
            }

            this.updateList();
        },

        getExistingModelFilter: function(filter) {
            var modelFilter = null;
            this.modelFilters.forEach(lang.hitch(this, function(existingFilter) {
                if (existingFilter.filter.name === filter.name) {
                    modelFilter = existingFilter;
                }
            }));

            return modelFilter;
        },

        contextChanged: function (context, sender) {

            this.model.context = context;
            if (!this.model.mainListEnabled()) {
                return;
            }

            this.widgetChange();
        },

        widgetChange: function() {
            this.model.populateData().then(lang.hitch(this, function (filters) {
                filters.forEach(lang.hitch(this, function (filter) {
                    var checkedItems = this.model.getCheckedItems(filter.name);

                    var modelFilter = this.getExistingModelFilter(filter);
                    var hasModelFilter = modelFilter !== null;
                    if (!hasModelFilter) {
                        this.own(require([filter.settings.filterPath], lang.hitch(this, function (filterClass) {
                            modelFilter = new filterClass();

                            modelFilter.setFilter(filter, checkedItems);
                            modelFilter.Write(lang.hitch(this, this.updateList));

                            var existingModelFilter = this.getExistingModelFilter(filter);
                            hasModelFilter = existingModelFilter !== null;
                            if (!hasModelFilter) {
                                this.facet.appendChild(modelFilter.domNode); // change this to something that always append in correct order
                                this.own(modelFilter);
                                this.modelFilters.push(modelFilter);
                            }
                        })));
                    } else {
                        modelFilter.setFilter(filter, checkedItems);
                        modelFilter.Write(lang.hitch(this, this.updateList));
                    }

                }));

                for (var i = 0; i < this.modelFilters.length; i++) {
                    var filterExist = false;

                    filters.forEach(lang.hitch(this, function (filter) {
                        if (filter.name === this.modelFilters[i].filter.name) {
                            filterExist = true;
                        }
                    }));

                    if (!filterExist) {
                        if (!!this.modelFilters[i].RemoveFilter()) {
                            this.facet.removeChild(this.modelFilters[i].domNode);
                            this.modelFilters[i].destroy();
                            this.modelFilters.splice(i, 1);
                            i--;
                        }
                    }
                }
            }));
        },

        updateList: function() {
            this.model.updateList(this, this.modelFilters, this.fasedEnabledPoint.checked, this.productGroupingPoint.checked, this.listMainArea.checked, this.list);
        }

    });
});