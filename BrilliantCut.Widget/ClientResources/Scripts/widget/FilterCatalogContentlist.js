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
        //    It uses facets in find to filter the content.
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
            var filterModel = this.profile.get("brilliantcut");
            if (!filterModel) {
                filterModel = {
                    valueString: "",
                    listingMode: "0",
                    productGrouped: false
                }
            }

            var currentCategory = id;
            if (originalQuery) {
                currentCategory = originalQuery.currentCategory;
            }

            var listingMode = filterModel.listingMode;
            if (listingMode !== "1") {
                listingMode = 0;
            }

            return {
                queryOptions: { ignore: ["query"], parentId: id, sort: (!simplified && !!onlyTopLevelChildren) ? [{ attribute: "typeSortIndex" }] : null },
                queryParameters: {
                    referenceId: id,
                    query: "getchildren",
                    filterModel: filterModel.valueString,
                    listingMode: listingMode,
                    productGrouped: filterModel.productGrouped,
                    simplified: simplified,
                    toplevel: onlyTopLevelChildren,
                    currentCategory: currentCategory,
                }
            };
        },
    });
});
