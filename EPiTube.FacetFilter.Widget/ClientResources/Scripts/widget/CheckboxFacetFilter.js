define([

// Dojo
    "dojo/_base/declare",

// Dijit
    "dijit/form/CheckBox",

//Commerce
    "./facetFilterBase"

], function (

// Doj
    declare,

// Dijit
    CheckBox,

//Commerce
    facetFilterBase

) {
    return declare([facetFilterBase], {
        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new CheckBox({
                name: filterOption.value,
                value: filterOption.value,
                checked: checked || filterOption.defaultValue,
                onChange: updateList
            }, filterContentName);
        },

        GetNoHitText: function (name, value) {
            return value + "(0)";
        },
    });
});