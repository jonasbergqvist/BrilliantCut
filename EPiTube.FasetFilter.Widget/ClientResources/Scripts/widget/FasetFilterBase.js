define([

// Dojo
    "dojo/dom-construct",
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
    domConstruct,
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

        setFilter: function (filter, checkedItems) {

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
                var id = this.GetId(existingItem.form.name, existingItem.form.value);
                if (id === name) {
                    checked = existingItem.form.checked;
                }
            }));

            return checked;
        },

        //setValue: function (dijitForm, filter, filterOption, checked) {
            
        //},

        GetId: function (name, value) {
            return value;
        },

        GetText: function(name, text, value) {
            return text;
        },

        GetValue: function(name, value) {
            return value;
        },

        RemoveFilter: function() {
            for (var i = 0; i < this.dijitForms.length; i++) {
                this.dijitForms.form.removeChild(i);
                i--;
                //this.filter.filterOptions.forEach(lang.hitch(this, function(filterOption) {
                //    for (var i = 0; i < this.dijitForms.length; i++) {
                //        var id = this.GetId(this.dijitForms[i].name, this.dijitForms[i].value);
                //        if (id === filterOption.id) {
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

        RemoveNonExistingAlternatives: function () {
            //var optionsToRemove = [];
            //this.dijitForms.forEach(lang.hitch(this, function(dijitForm) {
            for (var i = 0; i < this.dijitForms.length; i++) {
                var optionExists = false;
                this.filter.filterOptions.forEach(lang.hitch(this, function(filterOption) {
                    var id = this.GetId(this.dijitForms[i].form.name, this.dijitForms[i].form.value);
                    if (id === this.GetId(filterOption.id, filterOption.value)) {
                        optionExists = true;
                    }
                }));

                if (!optionExists) {
                    domConstruct.destroy(this.dijitForms[i].form.domNode.id);
                    domConstruct.destroy(this.dijitForms[i].label.id);
                    this.dijitForms[i].form.destroy();
                    this.dijitForms[i].label.remove();

                    this.dijitForms.splice(i, 1);
                    //this.dijitForms.removeChild(i);
                    i--;
                    //optionsToRemove.push(dijitForm);
                }
            }
            //}));

            //for (var i = 0; i < this.dijitForms.length; i++) {
            //    var alternativExist = false;
            //    this.alternativs.forEach(lang.hitch(this, function(alternative) {
            //        if (this.dijitForms[i].form.name === alternative.name) {
            //            alternativExist = true;
            //        }
            //    }));

            //    if (!alternativExist) {
            //        this.alternativs.removeChild(this.dijitForms[i].form.domNode);
            //        //this.alternativs[i].destroy();
            //        this.dijitForms.splice(i, 1);
            //        i--;
            //    }
            //};
        },

        Write: function (updateList) {
            this.caption.innerText = this.filter.filterContent.name;
            //this.caption.set('title', this.filter.filterContent.name);

            this.RemoveNonExistingAlternatives();

            this.filter.filterOptions.forEach(lang.hitch(this, function (filterOption) {
                var checked = false;
                this.checkedItems.forEach(lang.hitch(this, function (checkedItem) {
                    if (checkedItem === filterOption.id) {
                        checked = true;
                    }
                }));

                var dijitForm = null;
                this.dijitForms.forEach(lang.hitch(this, function (existingItems) {
                    var id = this.GetId(existingItems.form.name, existingItems.form.value);
                    if (id === this.GetId(filterOption.id, filterOption.value)) {
                        dijitForm = existingItems.form;
                        this.setFilter(this.filter, this.checkedItems);
                        existingItems.label.textContent = this.GetText(filterOption.id, filterOption.text, filterOption.value);
                    }
                }));

                if (dijitForm === null) {
                    dijitForm = this.CreateDijitForm(filterOption, this.filter.filterContent.name, this.filter.attribute, updateList);

                    if (dijitForm !== null) {
                        var label = dojo.create("label", { "for": filterOption.id, innerHTML: this.GetText(filterOption.id, filterOption.text, filterOption.value) });
                        this.dijitForms.push({ form: dijitForm, label: label });

                        this.own(dijitForm);
                        this.own(label);

                        var souroundedDiv = domConstruct.create("div");
                        souroundedDiv.appendChild(dijitForm.domNode);
                        souroundedDiv.appendChild(label);

                        this.alternativs.appendChild(souroundedDiv);
                    }
                }
            }));
        },

        CreateDijitForm: function (filterOption, filterContentName, updateList) {
            return null;
        }
    });
});