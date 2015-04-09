define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/keys",
    "dojo/on",

// Dijit
    "dijit/form/NumberTextBox",

//Commerce
    "./facetFilterBase"

], function (

// Doj
    declare,
    lang,
    keys,
    on,

// Dijit
    NumberTextBox,

//Commerce
    facetFilterBase

) {
    return declare([facetFilterBase], {
        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new NumberTextBox({
                name: filterOption.id,
                value: filterOption.value,
                onChange: function () {
                    updateList();
                }
            }, filterContentName);
        },

        IsChecked: function (name) {
            return this.GetValue(name) !== "";
        },

        GetId: function (name, value) {
            return name;
        },

        CheckedAtCreation: function(filterOption) {
            return false;
        }

        //GetValue: function (name, value) {
        //    this.dijitForms.forEach(lang.hitch(this, function(dijitForm) {
        //        if (dijitForm.form.name === name) {
        //            value = dijitForm.form.displayedValue;
        //        }
        //    }));

        //    return value;
        //}
    });
});