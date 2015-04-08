# BrilliantCut FacetFilter
BrilliantCut FacetFilter is a private project, mainly developed on the green tube in Stockholm between 06.35-07.00 in the morging.
The code has been created to show how nice EPiServer Commerce and EPiServer Find fit together, but anyone that wants to use this project in a site are more then welcome to do so. Anyone is also welcome to contribute to this project.

What is BrilliantCut FacetFilter?
---------------------------------
---------------------------------
BrilliantCut FacetFilter makes it possible to filter the catalog UI using facets using EPiServer Find. The project includes facets like language, market, and category. It also includes a free text search, and the possiblity to choose if the search should apply to children or descendants.
The more important feature in the API is the possiblity to create your own facets. Here is an example on a terms facet that is added for the property "Color" of content type "FashionVariant":

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Termsfacet<FashionVariant>(x => x.Color,
                        (builder, value) => builder.Or(x => x.Color.Match(value)));

Adding BrilliantCut FacetFilter to a site
--------------------------------------------
--------------------------------------------
There are two nuget packages in this project, "BrilliantCut.FacetFilter.Core", and "BrilliantCut.FacetFilter.Widget", that needs to be added to the site. "BrilliantCut.FacetFilter.Widget" depends on "BrilliantCut.FacetFilter.Core", so installing the widget package will automatically install the other one.

Update nuge packages
--------------------
Nuspec files exists for both nuget packages in the project. To update the nuget packages, simply use the commande nuget pack {fullPathToNuspecFile}, and the nuget package will be updated.

Adding nuget package
--------------------
Pull down the whole project to your computer, and add a new package source in visual studio that points to the project (https://docs.nuget.org/consume/Package-Manager-Dialog). Now it's easy to add BrilliantCut to your site using install-package BrilliantCut.FacetFilter.Widget in the package manager console.

Facets and filters
------------------
------------------
There is several build in facets and filters that can be used directly. 

ChildrenDescendentsFilter
-------------------------
This filter uses the "current content" in the catalog ui to descide the parent node to for which children or descendants should be listed.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<ChildrenDescendentsFilter>();

TextFilter
----------
A free text filter that searches on the "all" field. This free text search will also use "AnyWordBeginsWith" on the properties "Name" and "Code".

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<ChildrenDescendentsFilter>();

LanguageFilter
--------------
Facet to filter content on specific languages.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<LanguageFilter>();

MarketsFilter
-------------
Facet to filter content on specific markets.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<MarketsFilter>();

CategoryFilter
--------------
Facet to filter content on specific categories, which is parent to the listed content.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<CategoryFilter>();

Active
------
Filters on active content that is visible for the end user.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<IsActiveFilter>();

DefaultPriceFilter
------------------
Facet to filter content in a price range. This facet should not be used at the moment, because the index will not automatically be updated when prices are changed. This will be fixed after a user story has been done by the EPiServer Commerce team.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .DefaultPriceFilter();

InventoryFilter
---------------
Facet to filter content in an inventory range. This facet should not be used at the moment, because the index will not automatically be updated when inventory are changed. This will be fixed after a user story has been done by the EPiServer Commerce team.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .InventoryFilter();

Configure facets on the site
---------------------------
No facet or filter are added automatically to the widget. It's up to the developer to descide which ones that should be used on the site. The following line should be added to an Initialize method in an initialization module: 
                context.Locate.Advanced.GetInstance<FilterConfiguration>();

It's now easy to add facets in a selected order using the method "Facet<T>". Here is an example where several facets are added in a selected order:

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<ChildrenDescendentsFilter>()
                    .Facet<TextFilter>()
                    .Facet<LanguageFilter>()
                    .Facet<MarketsFilter>()
                    .Facet<CategoryFilter>();

Settings
--------
It's possible to change the default settings for the facets. Each build in facet has a default widget which will be used, but this can easaly be changed. The following example shows how to change the market filter to use radiobutton instead of checkboxes.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Facet<MarketsFilter>(new RadiobuttonFilterSetting());

There are also some intresting properties in the settings classes that can be set. The property "MaxFacetHits" will set the maximum facet items that are received from the index. The default number is 10, but can be changed to any number.

Another property that is useful on the "TextboxFilterSetting" is "Delay", which specifies how many milliseconds the client will wait until a request is done to the server when something is changed in the textbox.

Create you own facet
--------------------
--------------------
There are seveal ways to create a facet. The easiest onece are "terms facet", and "range facet".

Termsfacet<T>
-------------
It's very easy to create a terms facet for a property in a content type. This can be done in the fluent API in the same way as adding default facets.

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Termsfacet<FashionVariant>(x => x.Color,
                        (builder, value) => builder.Or(x => x.Color.Match(value)));

In this example we add a facet for the property "Color" in the content type "FashionVariant". The first argument specifies the property (Color), and the other argument tells "BrilliantCut" how to deal with several values that has been choosen for the facet in the UI. By using "builder.Or" we are doing an "or" for all selected values.

RangeFacet<TContent, TValue>
----------------------------
Creating a range facet is also pretty easy to do in the fluent API. The following example is the implementation of the "DefaultPriceFilter" extension method:

            return filterConfiguration.RangeFacet<VariationContent, double>(x => x.DefaultPrice(),
                (builder, values) => builder
                    .And(x => x.DefaultPrice().GreaterThan(values.Min() - 0.1))
                    .And(x => x.DefaultPrice().LessThan(values.Max() + 0.1)));

In the same way as the "terms facet", we are specifying the property or extension method in the first argument. The second argument is a bit different, where we use "GreaterThan", and "LessThan" to specify the interval for the facet.    

Creating complex facets or filter
---------------------------------
---------------------------------
It's possible to create a more complex facet by implementing an interface, or one of the two abstract base classes that exists in the project.

IFilterContent
--------------
The interface contains three methods and two properties.
* Name. The name of the facet.
* Description. The description of the facet.
* GetFilterOption. The options that will be send to the client.
* Filter. Filters the result using the selected values for the facet.
* AddFacetToQuery. Adds the facet to the query that will be send to the search index.

FilterContentBase<TContentData, TValueType>
-------------------------------------------
This abstract class contains generic versions of the methods on the interface. Here is an example of an implementation:

    [CheckboxFilter]
    public class MarketsFilter : FilterContentBase<EntryContentBase, string>
    {
        public override string Name
        {
            get { return "Markets"; }
        }

        public override ITypeSearch<EntryContentBase> Filter(IContent currentCntent, ITypeSearch<EntryContentBase> query, IEnumerable<string> values)
        {
            var marketFilter = SearchClient.Instance.BuildFilter<EntryContentBase>();
            marketFilter = values.Aggregate(marketFilter, (current, value) => current.Or(x => x.SelectedMarkets().Match(value)));

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode)
        {
            var facet = searchResults
                .TermsFacetFor<EntryContentBase>(x => x.SelectedMarkets()).Terms;

            return facet.Select(authorCount => new FilterOptionModel("marketing" + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count));
        }

        public override ITypeSearch<EntryContentBase> AddfacetToQuery(ITypeSearch<EntryContentBase> query, FacetFilterSetting setting)
        {
            return query.TermsFacetFor(x => x.SelectedMarkets());
        }
    }

FacetBase<T, TValue>
--------------------
This abstract class is base class for the "TermsFacet" and "RangeFacet". If you like to create another facet type that makes it possible to create facets directly in the fluent API, then this is the base class for you. Here is the implementation of the TermsFacet.

[CheckboxFilter]
    public class TermsFacet<T> : FacetBase<T, string>
        where T : IContent
    {
        public Func<FilterBuilder<T>, string, FilterBuilder<T>> Aggregate { get; set; }

        public override ITypeSearch<T> Filter(IContent currentCntent, ITypeSearch<T> query, IEnumerable<string> values)
        {
            var marketFilter = SearchClient.Instance.BuildFilter<T>();
            marketFilter = values.Aggregate(marketFilter, Aggregate);

            return query.Filter(marketFilter);
        }

        public override IEnumerable<IFilterOptionModel> GetFilterOptions(SearchResults<IFacetContent> searchResults, ListingMode mode)
        {
            var facet = searchResults
                .TermsFacetFor(PropertyValuesExpressionObject).Terms;

            return facet.Select(authorCount => new FilterOptionModel(Name + authorCount.Term, String.Format(CultureInfo.InvariantCulture, "{0} ({1})", authorCount.Term, authorCount.Count), authorCount.Term, false, authorCount.Count));
        }

        public override ITypeSearch<T> AddfacetToQuery(ITypeSearch<T> query, FacetFilterSetting setting)
        {
            var converted = Expression.Convert(PropertyValuesExpression.Body, typeof(string));

            var expression = Expression.Lambda<Func<T, string>>(converted, PropertyValuesExpression.Parameters);
            return query.TermsFacetFor(expression, request =>
            {
                if (setting.MaxFacetHits.HasValue)
                {
                    request.Size = setting.MaxFacetHits;
                }
            });
        }
    }
