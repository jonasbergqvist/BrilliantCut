define([

// Dojo
    "dojo/_base/declare",
    "dojo/Deferred",
    "dojo/aspect",
    "dojo/_base/lang",
    "dojo/dom-geometry",
    "dojo/dom-style",
    "dojo/dom-class",
    "dojo/html",
    "dojo/when",
    "dojo/_base/array",
    "dojox/layout/ScrollPane",
    "dojo/parser",
    "dojo/on",
    "dojo/string",

// Dijit
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/_WidgetBase",
    "dijit/layout/ContentPane",
    "dijit/form/CheckBox",
    "dijit/layout/BorderContainer",
    "dijit/layout/_LayoutWidget",

// epi shell
    "epi/shell/dgrid/util/misc",

//CMS 
    "epi-cms/_ContentContextMixin",
    "epi-cms/widget/ContentList",
    "epi-cms/core/ContentReference",
    "epi-cms/dgrid/formatters",

//Commerce
    "./viewmodel/facetFilterModel",

//Resources
    "epi/i18n!epi/cms/nls/episerver.shared.header",
    "dojo/text!./templates/facetFilter.html"
], function (

// Doj
    declare,
    Deferred,
    aspect,
    lang,
    geometry,
    domStyle,
    domClass,
    html,
    when,
    array,
    ScrollPane,
    parser,
    on,
    dojoString,

// Dijit
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    _WidgetBase,
    ContentPane,
    CheckBox,
    BorderContainer,
    _LayoutWidget,

// epi shell
    misc,

//CMS
    _ContentContextMixin,
    ContentList,
    ContentReference,
    formatters,

//Commerce
    facetFilterModel,

//Resources
    headingResources,
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

            var productGrouped = this.model.productGrouped();
            this.productGroupingPoint.set("checked", productGrouped);

            var listingMode = this.model.getListingMode();
            this.noListArea.set("checked", listingMode !== this.listMainArea.value && listingMode !== this.listWidgetArea.value);
            this.listMainArea.set("checked", listingMode === this.listMainArea.value);
            this.listWidgetArea.set("checked", listingMode === this.listWidgetArea.value);

            domClass.add(this.list.grid.domNode, "epi-thumbnailContentList");
        },

        layout: function () {

            var facetContainerh = this.model.getFacetContainerH();
            if (this._contentBox) {
                if (this.listWidgetArea.checked) {
                    if (facetContainerh > 50) {
                        this.listContainer.resize({
                            h: facetContainerh,
                            w: this._contentBox.w,
                            l: 0,
                            t: 0,
                        });
                    } else {
                        this.listContainer.resize({
                            h: 200,
                            w: this._contentBox.w,
                            l: 0,
                            t: 0,
                        });
                    }
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
                if (this.listContainer.h > 50) {
                    this.model.facetContainerH = this.listContainer.h;
                }
            }, true));
        },

        startup: function () {
            this.inherited(arguments);

            this._setFormatterForList();
            when(this.getCurrentContext(), lang.hitch(this, this.contextChanged));

            aspect.after(this.productGroupingPoint, "onChange", lang.hitch(this, function (checked) {
                this.updateList();
            }, true));

            aspect.after(this.noListArea, "onChange", lang.hitch(this, function (checked) {
                this.onlistAreaClick(checked);
            }, true));
            aspect.after(this.listMainArea, "onChange", lang.hitch(this, function (checked) {
                this.onlistAreaClick(checked);
            }, true));
            aspect.after(this.listWidgetArea, "onChange", lang.hitch(this, function (checked) {
                this.onlistAreaClick(checked);
            }, true));
        },

        onlistAreaClick: function (checked) {
            if (!checked) {
                return;
            }

            var unChecked = this.noListArea.checked;

            if (unChecked) {
                this.facet.style.display = "none";
                this.list.style.display = "none";
            } else {
                this.facet.style.display = "";
                this.list.style.display = "";
            }

            this.layout();
            this.updateList();
        },

        _setFormatterForList: function () {
            // summary: 
            //      Reset formatter for catalog hierarchical list.
            // tags:
            //      protected

            var grid = this.list.grid;
            // Reset formatter for catalog list to display both thumbnail and icon type identifier
            grid.formatters = [lang.hitch(this, this.catalogItemFormatter)];
            grid.configStructure();
        },

        catalogItemFormatter: function (value, object, node, options) {
            // summary: 
            //      Formatter for catalog list to display both thumbnail and icon type identifier.
            // tags:
            //      public

            var text = misc.htmlEncode(object.name);
            var title = misc.attributeEncode(this.getTitleSelector(object) || text);
            var returnValue = dojoString.substitute("${thumbnail} ${icon} ${text}", {
                thumbnail: formatters.thumbnail(this.getThumbnailSelector(object)),
                icon: formatters.contentIcon(object.typeIdentifier),
                text: misc.ellipsis(text, title)
            });

            node.innerHTML = returnValue;
            return returnValue;
        },

        getThumbnailSelector: function (item) {
            // summary: 
            //      Get thumbnail url from content item.
            // tags:
            //      public

            if (item && item.properties) {
                return item.properties.thumbnail;
            }
            return '';
        },

        getTitleSelector: function (item) {
            // summary: 
            //      Get title information from content item.
            // tags:
            //      public

            if (item) {
                var reference = new ContentReference(item.contentLink);
                if (reference) {
                    return dojoString.substitute("${name}, ${resourceId}: ${id} (${resourceType}: ${type})",
                        { name: item.name, resourceId: headingResources.id, id: reference.id, resourceType: headingResources.type, type: item.contentTypeName });
                }
            }
            return '';
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
            if (this.getListAreaValue() === this.noListArea.value) {
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

        updateList: function () {
            var selectedRadioValue = this.getListAreaValue();

            this.model.updateList(this, this.modelFilters, this.productGroupingPoint.checked, selectedRadioValue, this.list);
        },

        getListAreaValue: function () {
            //dojo.query("select[name=listArea]")[0].value;
            if (this.listMainArea.checked) {
                return this.listMainArea.value;
            }

            if (this.listWidgetArea.checked) {
                return this.listWidgetArea.value;
            }

            return this.noListArea.value;
        }

    });
});