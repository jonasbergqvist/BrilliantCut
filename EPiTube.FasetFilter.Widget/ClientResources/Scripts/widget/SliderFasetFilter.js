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

        slider: null,
        defaultValue: null,

        constructor: function () {
            this.inherited(arguments);
            this.defaultValue = [];
        },

        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            this.defaultValue.push({ key: filterOption.key, value: filterOption.value });

            this.slider = new HorizontalSlider({
                name: filterOption.key,
                value: this.getDefaultValue(filterOption.value),
                minimum: this.getMinMax(this.filter).min,
                maximum: this.getMinMax(this.filter).max,
                intermediateChanges: true,
                onChange: function (value) {
                    updateList();
                }
            }, filterContentName);

            this.setFilter(this.filter, this.checkedItems);
            return this.slider;
        },

        getDefaultValue: function(value) {
            if (value > 0) {
                return value;
            }

            return 0;
        }

        setFilter: function (filter, checkedItems) {
            this.inherited(arguments);

            if (this.slider) {
                var minMaxObject = this.getMinMax(filter);

                this.slider.set('minimum', minMaxObject.min);
                this.slider.set('maximum', minMaxObject.max);
            }
        },

        getMinMax: function (filter) {
            var min = 100000;
            var max = 0;
            filter.filterOptions.forEach(lang.hitch(this, function (filterOption) {
                if (filterOption.value > max) {
                    max = filterOption.value;
                }

                if (filterOption.value < min) {
                    min = filterOption.value;
                }
            }));

            if (min === 100000) {
                min = 0;
            }

            if (max === 0) {
                max = 1000;
            }

            return { min: min, max: max }
        },

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
                    value = Math.floor(existingItem.form.value);
                }
            }));

            return value;
        },

        GetText: function (name, value) {
            return this.GetValue(name, value);
        }
    });
});