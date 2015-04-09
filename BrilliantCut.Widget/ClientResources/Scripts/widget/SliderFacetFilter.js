define([

// Dojo
    "dojo/_base/declare",
    "dojo/_base/lang",

// Dijit
    "dijit/form/HorizontalSlider",

//Commerce
    "./facetFilterBase"

], function (

// Doj
    declare,
    lang,

// Dijit
    HorizontalSlider,

//Commerce
    facetFilterBase

) {
    return declare([facetFilterBase], {

        slider: null,
        defaultValue: null,

        constructor: function () {
            this.inherited(arguments);
            this.defaultValue = [];
        },

        CreateDijitForm: function (filterOption, checked, filterContentName, attribute, updateList) {
            this.defaultValue.push({ id: filterOption.id, value: filterOption.defaultValue });

            this.slider = new HorizontalSlider({
                name: filterOption.id,
                minimum: this.getMinMax(this.filter).min,
                maximum: this.getMinMax(this.filter).max,
                intermediateChanges: true,
                onChange: function (value) {
                    updateList();
                }
            }, filterContentName);

            return this.slider;
        },

        Write: function () {
            this.inherited(arguments);

            if (this.slider) {
                var minMaxObject = this.getMinMax(this.filter);
                //var defaultMinMaxObject = this.getDefaultMinMax(this.filter);

                //var currentMinimum = this.slider.get('minimum');
                //if (minMaxObject.min !== defaultMinMaxObject.min || currentMinimum === "min") {
                    this.slider.set('minimum', minMaxObject.min);

                    if (this.slider.value <= minMaxObject.min) {
                        this.slider.set('value', minMaxObject.min);
                    }
                //}

                //var currentMaximum = this.slider.get('maximum');
                //if (minMaxObject.max !== defaultMinMaxObject.max || currentMaximum === "max") {
                    this.slider.set('maximum', minMaxObject.max);

                    if (this.slider.value >= minMaxObject.max) {
                        this.slider.set('value', minMaxObject.max);
                    }
                //}
            }
        },

        CheckedAtCreation: function(filterOption) {
            return false;
        },

        //setFilter: function (filter, checkedItems) {
        //    this.inherited(arguments);

        //    if (this.slider) {
        //        var minMaxObject = this.getMinMax(filter);
        //        var defaultMinMaxObject = this.getDefaultMinMax(filter);

        //        var currentMinimum = this.slider.get('minimum');
        //        if (minMaxObject.min !== defaultMinMaxObject.min || currentMinimum === "min") {
        //            //this.slider.set('minimum', minMaxObject.min);
        //            this.slider.attr('minimum', minMaxObject.min);

        //            if (this.slider.value < minMaxObject.min) {
        //                this.slider.value = minMaxObject.min;
        //            }
        //        }

        //        var currentMaximum = this.slider.get('maximum');
        //        if (minMaxObject.max !== defaultMinMaxObject.max || currentMaximum === "max") {
        //            //this.slider.set('maximum', minMaxObject.max);
        //            this.slider.attr('maximum', minMaxObject.max);

        //            if (this.slider.value > minMaxObject.max) {
        //                this.slider.value = minMaxObject.max;
        //            }
        //        }
        //    }
        //},

        //getDefaultMinMax: function(filter) {
        //    var min = -1;
        //    var max = -1;

        //    filter.filterOptions.forEach(lang.hitch(this, function(filterOption) {
        //        if (filterOption.defaultValue > max) {
        //            max = filterOption.defaultValue;
        //        }

        //        if (filterOption.defaultValue < min || min === -1) {
        //            min = filterOption.defaultValue;
        //        }
        //    }));

        //    return { min: min, max: max }
        //},

        getMinMax: function (filter) {
            var min = -1;
            var max = -1;

            filter.filterOptions.forEach(lang.hitch(this, function(filterOption) {
                if (filterOption.value > max) {
                    max = filterOption.value;
                }

                if (filterOption.value < min || min === -1) {
                    min = filterOption.value;
                }
            }));

            if (min === -1) {
                min = "min";
            }

            if (max === -1) {
                max = "max";
            }

            return { min: min, max: max }
        },

        IsChecked: function (name) {
            var returnValue = false;
            this.defaultValue.forEach(lang.hitch(this, function (defaultKeyValue) {
                if (this.GetValue(defaultKeyValue.id, defaultKeyValue.value) !== defaultKeyValue.value) {
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
        },
    });
});