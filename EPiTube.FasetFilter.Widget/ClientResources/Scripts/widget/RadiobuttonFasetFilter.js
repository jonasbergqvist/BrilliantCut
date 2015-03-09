define([

// Dojo
    "dojo/_base/declare",

// Dijit
    "dijit/form/RadioButton",

//Commerce
    "./FasetFilterBase"

], function (

// Doj
    declare,

// Dijit
    RadioButton,

//Commerce
    FasetFilterBase

) {
    return declare([FasetFilterBase], {
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