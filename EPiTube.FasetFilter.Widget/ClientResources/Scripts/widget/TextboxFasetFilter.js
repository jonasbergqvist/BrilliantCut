define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/keys",
    "dojo/on",

// Dijit
    "dijit/form/TextBox",

//Commerce
    "./FasetFilterBase"

], function (

// Doj
    declare,
    lang,
    keys,
    on,

// Dijit
    TextBox,

//Commerce
    FasetFilterBase

) {
    return declare([FasetFilterBase], {
        timeoutId: 0,

        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new TextBox({
                name: filterOption.id,
                value: '',
                onKeyDown: function () {
                    clearTimeout(this.timeoutId);
                    this.timeoutId = setTimeout(updateList, attribute.delay);
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
                    value = dijitForm.form.displayedValue;
                }
            }));

            return value;
        }//,

        //GetText: function(name, value) {
        //    return "";
        //}
    });
});