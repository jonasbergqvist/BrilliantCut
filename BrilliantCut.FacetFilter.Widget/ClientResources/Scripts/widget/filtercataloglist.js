define([
// Dojo
    "dojo",
    "dojo/_base/declare",
    "dojo/_base/json",

// CMS
    "epi-cms/Profile",
    "epi/dependency",

//commerce
    "epi-ecf-ui/widget/CatalogList"

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
 
        fetchData: function (context) {
            // summary:
            //		Fetches data by setting a query on the grid. A getrelations query will be performed on the store.
            // tags:
            //		protected

            var filterModel = this.profile.get("brilliantcut");
            if (!filterModel) {
                filterModel = {
                    valueString: "",
                    listingMode: "0",
                    productGrouped: false
                }
            }

            var listingMode = filterModel.listingMode;
            if (listingMode !== "1") {
                listingMode = 0;
            }

            var queryOptions = { ignore: ["query"], parentId: context.id, sort: [{ attribute: "name" }] };
            var queryParameters = {
                referenceId: context.id,
                query: "getchildren",
                filterModel: filterModel.valueString,
                listingMode: listingMode,
                productGrouped: filterModel.productGrouped
            };

            this.grid.set("query", queryParameters, queryOptions);
        }

    });
});
