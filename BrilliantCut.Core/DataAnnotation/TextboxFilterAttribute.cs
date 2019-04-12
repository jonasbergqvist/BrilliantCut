// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextboxFilterAttribute.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.DataAnnotation
{
    using System;

    using BrilliantCut.Core.FilterSettings;

    /// <summary>
    /// Class TextboxFilterAttribute. This class cannot be inherited.
    /// Implements the <see cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    /// </summary>
    /// <seealso cref="BrilliantCut.Core.DataAnnotation.FacetFilterAttribute" />
    /// <remarks>Delay = 1000</remarks>

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TextboxFilterAttribute : FacetFilterAttribute
    {
        private readonly int delay;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextboxFilterAttribute"/> class.
        /// </summary>
        public TextboxFilterAttribute()
            : this(1000)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextboxFilterAttribute"/> class.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public TextboxFilterAttribute(int delay)
        {
            this.delay = delay;
            this.Setting = new TextboxFilterSetting(delay: delay);
        }

        /// <summary>
        /// Gets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public int Delay
        {
            get
            {
                return this.delay;
            }
        }
    }
}