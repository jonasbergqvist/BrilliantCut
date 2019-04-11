// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeFacet.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System;

    using EPiServer.Core;

    /// <summary>
    /// Class DateTimeFacet.
    /// Implements the <see cref="RangeFacet{TContent,TValue}" />
    /// </summary>
    /// <typeparam name="TContent">The type of the t content.</typeparam>
    /// <seealso cref="RangeFacet{TContent,TValue}" />
    public class DateTimeFacet<TContent> : RangeFacet<TContent, DateTime>
        where TContent : IContent
    {
    }
}