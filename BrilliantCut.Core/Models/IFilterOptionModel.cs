// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterOptionModel.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    public interface IFilterOptionModel
    {
        int Count { get; }

        object DefaultValue { get; }

        string Id { get; }

        string Text { get; }

        object Value { get; }
    }
}