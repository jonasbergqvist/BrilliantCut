define([
// Dojo
    "dojo",
    "dojo/_base/declare",
    "dojo/_base/json",

// CMS
    "epi-cms/Profile",
    "epi/dependency",

//commerce
    "epi-ecf-ui/widget/CatalogContentList"

], function (
// Dojo
    dojo,
    declare,
    json,

// CMS
    Profile,
    dependency,

//commerce
    CatalogList
) {

    return declare([CatalogList], {
        // summary:
        //    Lists the content of the catalog root, with filter support
        // description:
        //    This is the widget that lists the content of the catalog root.
        //    It uses Fasets in find to filter the content.
        // tags:
        //    public

        profile: null,

        postscript: function () {
            // summary:
            //      Post properties mixin handler.
            // description:
            //		Set up model and resource for template binding.
            // tags:
            //		protected

            this.inherited(arguments);

            this.profile = dependency.resolve("epi.shell.Profile");
        },

        _getQueryOptions: function (id, simplified, onlyTopLevelChildren, originalQuery) {
            var currentCategory = id;
            if (originalQuery) {
                currentCategory = originalQuery.currentCategory;
            }

            var filterModel = this.profile.get("epitubefilter");

            return {
                queryOptions: { ignore: ["query"], parentId: id, sort: (!simplified && !!onlyTopLevelChildren) ? [{ attribute: "typeSortIndex" }] : null },
                queryParameters: {
                    referenceId: id,
                    query: "getchildren",
                    market: this.marketSelector ? this.marketSelector.value : null,
                    includeProperties: true,
                    allLanguages: true,
                    toplevel: onlyTopLevelChildren,
                    currentCategory: currentCategory,
                    simplified: simplified,
                    keepversion: true,
                    filterModel: filterModel.valueString,
                    filterEnabled: filterModel.enabled,
                    productGrouped: filterModel.productGrouped
                }
            };
        },
    });
});
