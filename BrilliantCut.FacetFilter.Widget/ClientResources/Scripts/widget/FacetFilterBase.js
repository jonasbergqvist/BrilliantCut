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
    "./viewmodel/facetFilterBaseModel",

//Resources
    "dojo/text!brilliantcut/widget/templates/facetFilterBase.html",

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
    facetFilterModel,

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
        },

        setFilter: function (filter, checkedItems) {
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

        GetId: function (name, value) {
            return value;
        },

        GetText: function(name, text, value, count) {
            return text;
        },

        GetNoHitText: function(name, value) {
            return value;
        },

        GetValue: function(name, value) {
            return value;
        },

        RemoveFilter: function () {
            if (this.checkedItems.length > 0) {
                this.UpdateTextForRemovedItems();
                return false;
            }

            for (var i = 0; i < this.dijitForms.length; i++) {
                domConstruct.destroy(this.dijitForms[i].form.domNode.id);
                domConstruct.destroy(this.dijitForms[i].label.id);
                this.dijitForms[i].form.destroy();
                this.dijitForms[i].label.remove();

                this.dijitForms.splice(i, 1);
                //this.dijitForms.form.removeChild(i);
                i--;
            }

            for (var i = 0; i < this.alternativs.length; i++) {
                this.alternativs.removeChild(i);
                i--;
            }

            return true;
        },

        RemoveNonExistingAlternatives: function () {

            for (var i = 0; i < this.dijitForms.length; i++) {
                var optionExists = false;
                var optionChecked = false;

                var id = this.GetId(this.dijitForms[i].form.name, this.dijitForms[i].form.value);
                this.filter.filterOptions.forEach(lang.hitch(this, function(filterOption) {
                    if (id === this.GetId(filterOption.id, filterOption.value)) {
                        optionExists = true;
                    }
                }));

                this.checkedItems.forEach(lang.hitch(this, function(checkedItem) {
                    if (id === this.GetId(checkedItem.name, checkedItem.value)) {
                        optionChecked = true;
                    }
                }));

                if (!optionExists) {
                    if (!optionChecked) {
                        domConstruct.destroy(this.dijitForms[i].form.domNode.id);
                        domConstruct.destroy(this.dijitForms[i].label.id);
                        this.dijitForms[i].form.destroy();
                        this.dijitForms[i].label.remove();

                        this.dijitForms.splice(i, 1);
                        i--;
                    } else {
                        this.dijitForms[i].label.textContent = this.GetNoHitText(this.dijitForms[i].form.name, this.dijitForms[i].form.value);
                    }
                }
            }

            return true;
        },

        UpdateTextForRemovedItems: function() {
            this.dijitForms.forEach(lang.hitch(this, function (existingItems) {

                var id = this.GetId(existingItems.form.name, existingItems.form.value);
                this.checkedItems.forEach(lang.hitch(this, function (checkedItem) {
                    if (id === this.GetId(checkedItem.name, checkedItem.value)) {
                        existingItems.label.textContent = this.GetNoHitText(existingItems.form.name, existingItems.form.value);
                    }
                }));
            }));
        },

        Write: function (updateList) {
            this.caption.innerText = this.filter.name;

            if (!this.RemoveNonExistingAlternatives()) {
                return;
            }

            this.filter.filterOptions.forEach(lang.hitch(this, function (filterOption) {

                var dijitForm = null;
                this.dijitForms.forEach(lang.hitch(this, function (existingItems) {
                    var id = this.GetId(existingItems.form.name, existingItems.form.value);
                    if (id === this.GetId(filterOption.id, filterOption.value)) {
                        dijitForm = existingItems.form;
                        this.setFilter(this.filter, this.checkedItems);
                        existingItems.label.textContent = this.GetText(filterOption.id, filterOption.text, filterOption.value, filterOption.count);
                    }
                }));

                if (dijitForm === null) {
                    var optionChecked = this.CheckedAtCreation(filterOption);

                    dijitForm = this.CreateDijitForm(filterOption, optionChecked, this.filter.name, this.filter.settings, updateList);

                    if (dijitForm !== null) {
                        var label = dojo.create("label", { "for": filterOption.id, innerHTML: this.GetText(filterOption.id, filterOption.text, filterOption.value, filterOption.count) });
                        this.dijitForms.push({ form: dijitForm, label: label });

                        this.own(dijitForm);
                        this.own(label);

                        var souroundedDiv = domConstruct.create("div");
                        souroundedDiv.appendChild(dijitForm.domNode);
                        souroundedDiv.appendChild(label);

                        this.alternativs.appendChild(souroundedDiv);
                        dijitForm.startup();
                    }
                }
            }));
        },

        CreateDijitForm: function (filterOption, checked, filterContentName, updateList) {
            return null;
        },

        CheckedAtCreation: function (filterOption) {
            var optionChecked = false;
            var id = this.GetId(filterOption.id, filterOption.value);
            this.checkedItems.forEach(lang.hitch(this, function (checkedItem) {
                if (id === this.GetId(checkedItem.name, checkedItem.value)) {
                    optionChecked = true;
                }
            }));

            return optionChecked;
        }
    });
});