// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterOptionModel.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Models
{
    /// <summary>
    /// Class FilterOptionModel.
    /// Implements the <see cref="BrilliantCut.Core.Models.IFilterOptionModel" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.Models.IFilterOptionModel" />
    public class FilterOptionModel : IFilterOptionModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterOptionModel"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="count">The count.</param>
        public FilterOptionModel(string id, string text, object value, object defaultValue, int count)
        {
            this.Id = id;
            this.Text = text;
            this.Value = value;
            this.DefaultValue = defaultValue;
            this.Count = count;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public object DefaultValue { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; }
    }
}