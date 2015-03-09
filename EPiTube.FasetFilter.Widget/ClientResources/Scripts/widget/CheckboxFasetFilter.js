define([

// Dojo
    "dojo/_base/declare",

// Dijit
    "dijit/form/CheckBox",

//Commerce
    "./FasetFilterBase"

], function (

// Doj
    declare,

// Dijit
    CheckBox,

//Commerce
    FasetFilterBase

) {
    return declare([FasetFilterBase], {
        CreateDijitForm: function (filterOption, filterContentName, attribute, updateList) {
            return new CheckBox({
                name: filterOption.value,
                value: filterOption.value,
                checked: filterOption.defaultValue,
                onChange: updateList
            }, filterContentName);
        }
    });
});