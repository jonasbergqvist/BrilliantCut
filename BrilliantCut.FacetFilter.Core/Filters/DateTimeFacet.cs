using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Find;
using EPiServer.Find.Framework;
using BrilliantCut.FacetFilter.Core.Models;

namespace BrilliantCut.FacetFilter.Core.Filters
{
    public class DateTimeFacet<TContent> : RangeFacet<TContent, DateTime>
        where TContent : IContent
    {
    }
}
