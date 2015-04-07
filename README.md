# BrilliantCut
BrilliantCut FacetFilter is a private project created by Jonas Bergqvist, mainly developed on the green tube in Stockholm between 06.35-07.00 in the morging.
The code has been created to show how nice EPiServer Commerce and EPiServer Find fits together, but anyone that wants to use this project in a project are more then welcome to do so. Anyone is also welcome to contribute to the project.

What is BrilliantCut FacetFilter?
---------------------------------
BrilliantCut FacetFilter makes it possible to filter the catalog UI using facets. The project includes facets like language, market, and category. It also includes a free text search, and the possiblity to choose if the search should apply to children or descendants.
The mose important feature in the API is the possiblity to create your own facets. Here is an example on a terms facet that is added:

                context.Locate.Advanced.GetInstance<FilterConfiguration>()
                    .Termsfacet<FashionVariant>(x => x.Color,
                        (builder, value) => builder.Or(x => x.Color.Match(value)));

Adding nuget package
--------------------

Configure filters
-----------------

* ChildrenDescendentsFilter: 
* TextFilter: 
* LanguageFilter: 
* MarketsFilter: 
* CategoryFilter: 
* DefaultPriceFilter: 
* InventoryFilter: 
* Termsfacet<T>: 
* RangeFacet<TContent, TValue>: 

Creating complex facets or filter
---------------------------------
* IFilterContent: 
* FilterContentBase<TContentData, TValueType>: 
* FacetBase<T, TValue>: 
