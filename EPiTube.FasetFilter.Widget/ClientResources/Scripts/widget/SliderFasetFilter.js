define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

// Dijit
    "dijit/form/HorizontalSlider",

//Commerce
    "./FasetFilterBase"

], function (

// Doj
    declare,
    lang,

// Dijit
    HorizontalSlider,

//Commerce
    FasetFilterBase

) {
    return declare([FasetFilterBase], {

        //slider: null,
        defaultValue: null,

        constructor: function () {
            this.inherited(arguments);
            this.defaultValue = [];
        },

        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            this.defaultValue.push({ key: filterOption.key, value: filterOption.value });

            return new HorizontalSlider({
                name: filterOption.key,
                value: filterOption.value,
                minimum: 0,
                maximum: 1000,
                intermediateChanges: true,
                //style: "width:100px;",
                onChange: function(value) {
                    //dojo.byId("sliderValue").value = value;
                    updateList();
                }
            }, filterContentName);

            //return this.slider;
        },

        //setFilter: function(filter, checkedItems) {
        //    this.inherited(arguments);

        //    var min = 10000;
        //    var max = 0;
        //    filter.filterOptions.forEach(lang.hitch(this, function (filterOption) {
        //        if (filterOption.value > max) {
        //            max = filterOption.value;
        //        }

        //        if (filterOption.value < min) {
        //            min = filterOption.value;
        //        }
        //    }));

        //    //this.slider.minimum = min;
        //    //this.slider.maximum = max;
        //},

        IsChecked: function (name) {
            var returnValue = false;
            this.defaultValue.forEach(lang.hitch(this, function (defaultKeyValue) {
                if (this.GetValue(defaultKeyValue.key, defaultKeyValue.value) !== defaultKeyValue.value) {
                    returnValue = true;
                }
            }));

            return returnValue;
        },

        GetId: function (name, value) {
            return name;
        },

        GetValue: function (name, value) {
            this.dijitForms.forEach(lang.hitch(this, function (existingItem) {
                if (existingItem.form.name === name) {
                    value = existingItem.form.value;
                }
            }));

            return value;
        }
    });
});