define([

// Dojo
    "dojo/_base/declare",

// Dijit
    "dijit/form/CheckBox",

//Commerce
    "./FasctFilterBase"

], function (

// Doj
    declare,

// Dijit
    CheckBox,

//Commerce
    FacetFilterBase

) {
    return declare([FacetFilterBase], {
        CreateDijitForm: function (filterOption, checked, updateList) {
            return new CheckBox({
                //id: filterOption.key,
                name: filterOption.key,
                checked: checked,
                onChange: updateList
            }, "checkBox");
        }
    });
});