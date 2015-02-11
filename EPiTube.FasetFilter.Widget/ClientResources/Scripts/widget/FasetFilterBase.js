define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/html",
    "dojo/on",

// Dijit
    "dijit/_TemplatedMixin",
    "dijit/_WidgetBase",

//CMS 
    "epi-cms/_ContentContextMixin",

//Commerce
    "./viewmodel/FasetFilterBaseModel",

//Resources
    "dojo/text!epitubefasetfilter/widget/templates/FasetFilterBase.html",

    "dojo/domReady!"
], function (

// Doj
    declare,
    lang,
    html,
    on,

// Dijit
    _TemplatedMixin,
    _WidgetBase,

//CMS
    _ContentContextMixin,

//Commerce
    FasetFilterModel,

//Resources
    template
) {
    return declare([_WidgetBase, _TemplatedMixin, _ContentContextMixin], {
        templateString: template,
        filter: null,
        dijitForms: null,
        checkedItems: null,

        constructor: function () {
            this.dijitForms = [];
            this.checkedItems = [];
        },

        postscript: function() {
            // summary:
            //      Post properties mixin handler.
            // description:
            //		Set up model and resource for template binding.
            // tags:
            //		protected

            this.inherited(arguments);

            on(this.caption, "click", lang.hitch(this, function (e) {
                if (this.alternativs.style.display === "none") {
                    this.alternativs.style.display = "";
                    this.caption.class = "dgrid-expando-icon dgrid-expando-arrow";
                } else {
                    this.alternativs.style.display = "none";
                    this.caption.class = "dgrid-expando-icon dgrid-expando-arrow ui-icon-triangle-1-se";
                }
            }));

        //    var toggler = new Toggler({
        //        node: "alternativs",
        //        //showFunc: coreFx.wipeIn,
        //        //hideFunc: coreFx.wipeOut
        //    });
        //    on(dom.byId("hideButton"), "click", function (e) {
        //        toggler.hide();
        //    });
        //    on(dom.byId("showButton"), "click", function (e) {
        //        toggler.show();
        //    });
        },

        SetFilter: function (filter, checkedItems) {

            if (this.filter === null) {
                //var toggler = new Toggler({
                //    node: this.alternativs,
                //    //showFunc: coreFx.wipeIn,
                //    //hideFunc: coreFx.wipeOut
                //});

                //on(this.hButton, "click", function (e) {
                //    toggler.collapse();
                //});
                //on(this.sButton, "click", function (e) {
                //    toggler.expand();
                //});
            }

            this.filter = filter;
            this.checkedItems = checkedItems;
        },

        IsChecked: function (name) {
            var checked = false;
            this.dijitForms.forEach(lang.hitch(this, function (existingItem) {
                var id = this.GetId(existingItem.name, existingItem.value);
                if (id === name) {
                    checked = existingItem.checked;
                }
            }));

            return checked;
        },

        GetId: function (name, value) {
            return value;
        },

        GetText: function(name, value) {
            return this.GetId(name, value);
        },

        GetValue: function(name, value) {
            return value;
        },

        RemoveFilter: function() {
            for (var i = 0; i < this.dijitForms.length; i++) {
                this.dijitForms.removeChild(i);
                i--;
                //this.filter.filterOptions.forEach(lang.hitch(this, function(filterOption) {
                //    for (var i = 0; i < this.dijitForms.length; i++) {
                //        var id = this.GetId(this.dijitForms[i].name, this.dijitForms[i].value);
                //        if (id === filterOption.key) {
                //            //this.alternativs.removeChild(this.dijitForms[i].domNode);

                //            //this.alternativs[i].destroy();
                //            this.dijitForms.splice(i, 1);
                //            i--;
                //        }
                //    }
                //}));
            }

            for (var i = 0; i < this.alternativs.length; i++) {
                this.alternativs.removeChild(i);
                i--;
            }
        },

        RemoveNonExistingAlternatives: function() {
            for (var i = 0; i < this.dijitForms.length; i++) {
                var alternativExist = false;
                this.alternativs.forEach(lang.hitch(this, function(alternative) {
                    if (this.dijitForms[i].name === alternative.name) {
                        alternativExist = true;
                    }
                }));

                if (!alternativExist) {
                    this.alternativs.removeChild(this.dijitForms[i].domNode);
                    //this.alternativs[i].destroy();
                    this.dijitForms.splice(i, 1);
                    i--;
                }
            };
        },

        Write: function (updateList) {
            this.caption.innerText = this.filter.filterContent.name;
            //this.caption.title = this.filter.filterContent.name;
            //this.caption.set('title', this.filter.filterContent.name);

            this.filter.filterOptions.forEach(lang.hitch(this, function (filterOption) {
                var checked = false;
                this.checkedItems.forEach(lang.hitch(this, function (checkedItem) {
                    if (checkedItem === filterOption.key) {
                        checked = true;
                    }
                }));

                var dijitForm = null;
                this.dijitForms.forEach(lang.hitch(this, function (existingItems) {
                    var id = this.GetId(existingItems.name, existingItems.value);
                    if (id === filterOption.key) {
                        dijitForm = existingItems;
                    }
                }));

                if (dijitForm === null) {
                    dijitForm = this.CreateDijitForm(filterOption, checked, this.filter.filterContent.name, this.filter.attribute, updateList);

                    if (dijitForm !== null) {
                        this.dijitForms.push(dijitForm);
                        this.own(dijitForm);

                        var label = dojo.create("label", { "for": filterOption.key, innerHTML: this.GetText(filterOption.key, filterOption.value) });
                        this.own(label);

                        this.alternativs.appendChild(dijitForm.domNode);
                        this.alternativs.appendChild(label);
                        this.alternativs.appendChild(document.createElement('br'));
                    }
                }
            }));
        },

        CreateDijitForm: function (filterOption, checked, filterContentName, updateList) {
            return null;
        }
    });
});