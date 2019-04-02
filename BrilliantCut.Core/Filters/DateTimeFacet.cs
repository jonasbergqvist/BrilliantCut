// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeFacet.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System;

    using EPiServer.Core;

    public class DateTimeFacet<TContent> : RangeFacet<TContent, DateTime>
        where TContent : IContent
    {
    }
}