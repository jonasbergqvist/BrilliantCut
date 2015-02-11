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