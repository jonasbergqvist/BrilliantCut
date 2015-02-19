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
        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new CheckBox({
                name: filterOption.value,
                value: filterOption.value,
                checked: checked,
                onChange: updateList
            }, filterContentName);
        },

        GetText: function (name, value) {
            return name;
        },

        //GetId: function (name, value) {
        //    return name;
        //}
    });
});