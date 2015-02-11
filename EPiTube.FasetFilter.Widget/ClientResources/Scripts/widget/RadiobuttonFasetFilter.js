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
        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new RadioButton({
                //id: filterOption.key,
                name: filterContentName,
                value: filterOption.key,
                checked: checked,
                onChange: updateList
            }, filterContentName);
        }
    });
});