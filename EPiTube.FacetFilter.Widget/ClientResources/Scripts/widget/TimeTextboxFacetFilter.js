define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/keys",
    "dojo/on",

// Dijit
    "dijit/form/TimeTextBox",

//Commerce
    "./facetFilterBase"

], function (

// Doj
    declare,
    lang,
    keys,
    on,

// Dijit
    TimeTextBox,

//Commerce
    facetFilterBase

) {
    return declare([facetFilterBase], {
        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new TimeTextBox({
                name: filterOption.id,
                value: filterOption.value,
                constraints: {
                    timePattern: 'HH:mm:ss',
                    clickableIncrement: 'T00:15:00',
                    visibleIncrement: 'T00:15:00',
                    visibleRange: 'T01:00:00'
                },
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

        GetValue: function (name, value) {
            this.dijitForms.forEach(lang.hitch(this, function(dijitForm) {
                if (dijitForm.form.name === name) {
                    value = dijitForm.form.displayedValue;
                }
            }));

            return value;
        },

        CheckedAtCreation: function (filterOption) {
            return false;
        }
    });
});