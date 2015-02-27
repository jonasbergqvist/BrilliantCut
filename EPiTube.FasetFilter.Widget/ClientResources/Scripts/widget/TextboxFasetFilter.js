define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

// Dijit
    "dijit/form/TextBox",

//Commerce
    "./FasetFilterBase"

], function (

// Doj
    declare,
    lang,

// Dijit
    TextBox,

//Commerce
    FasetFilterBase

) {
    return declare([FasetFilterBase], {
        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new TextBox({
                name: filterOption.id,
                value: '',
                onChange: function() {
                    setTimeout(updateList, attribute.delay);
                }
            }, filterContentName);
        },

        IsChecked: function (name) {
            return true;
        },

        GetId: function (name, value) {
            return name;
        },

        GetValue: function (name, value) {
            this.dijitForms.forEach(lang.hitch(this, function(dijitForm) {
                if (dijitForm.form.name === name) {
                    value = dijitForm.form.value;
                }
            }));

            return value;
        }//,

        //GetText: function(name, value) {
        //    return "";
        //}
    });
});