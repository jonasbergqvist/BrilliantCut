// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterOptionModel.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    /// <summary>
    /// Interface IFilterOptionModel
    /// </summary>
    public interface IFilterOptionModel
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>The default value.</value>
        object DefaultValue { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string Id { get; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; }
    }
}