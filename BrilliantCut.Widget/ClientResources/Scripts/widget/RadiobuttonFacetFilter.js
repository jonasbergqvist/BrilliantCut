define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

// Dijit
    "dijit/form/RadioButton",

//Commerce
    "./facetFilterBase"

], function (

// Doj
    declare,
    lang,

// Dijit
    RadioButton,

//Commerce
    facetFilterBase

) {
    return declare([facetFilterBase], {
        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            return new RadioButton({
                name: filterContentName,
                value: filterOption.value,
                checked: checked,
                onChange: updateList
            }, filterContentName);
        },

        CheckedAtCreation: function (filterOption) {
            if (this.checkedItems.length === 0) {
                return filterOption.defaultValue;
            }

            var optionChecked = false;
            var id = this.GetId(filterOption.id, filterOption.value);
            this.checkedItems.forEach(lang.hitch(this, function (checkedItem) {
                if (id === this.GetId(checkedItem.name, checkedItem.value)) {
                    optionChecked = true;
                }
            }));

            return optionChecked;
        }
    });
});