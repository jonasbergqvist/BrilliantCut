define([

// Dojo
    "dojo/_base/declare",

// Dijit
    "dijit/form/RadioButton",

//Commerce
    "./facetFilterBase"

], function (

// Doj
    declare,

// Dijit
    RadioButton,

//Commerce
    facetFilterBase

) {
    return declare([facetFilterBase], {
        CreateDijitForm: function (filterOption, filterContentName, attribute, updateList) {
            return new RadioButton({
                name: filterContentName,
                value: filterOption.value,
                checked: filterOption.defaultValue,
                onChange: updateList
            }, filterContentName);
        }
    });
});