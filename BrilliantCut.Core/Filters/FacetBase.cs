// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FacetBase.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core.Filters
{
    using System;
    using System.Linq.Expressions;

    using EPiServer.Core;

    /// <summary>
    /// Class FacetBase.
    /// Implements the <see cref="FilterContentBase{T, TValue}" />
    /// </summary>
    /// <typeparam name="T">The type of content</typeparam>
    /// <typeparam name="TValue">The type of the t value.</typeparam>
    /// <seealso cref="FilterContentBase{T, TValue}" />
    public abstract class FacetBase<T, TValue> : FilterContentBase<T, TValue>
        where T : IContent
    {
        /// <summary>
        /// The name
        /// </summary>
        private string name;

        /// <summary>
        /// The property values expression object
        /// </summary>
        private Expression<Func<T, object>> propertyValuesExpressionObject;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return this.name ?? (this.name = GetPropertyName(expression: this.PropertyValuesExpression.Body));
            }
        }

        /// <summary>
        /// Gets or sets the property values expression.
        /// </summary>
        /// <value>The property values expression.</value>
        public Expression<Func<T, TValue>> PropertyValuesExpression { get; set; }

        /// <summary>
        /// Gets the property values expression object.
        /// </summary>
        /// <value>The property values expression object.</value>
        public Expression<Func<T, object>> PropertyValuesExpressionObject
        {
            get
            {
                if (this.propertyValuesExpressionObject != null)
                {
                    return this.propertyValuesExpressionObject;
                }

                UnaryExpression converted = Expression.Convert(
                    expression: this.PropertyValuesExpression.Body,
                    type: typeof(object));

                this.propertyValuesExpressionObject = Expression.Lambda<Func<T, object>>(
                    body: converted,
                    parameters: this.PropertyValuesExpression.Parameters);

                return this.propertyValuesExpressionObject;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The property name.</returns>
        /// <exception cref="NotSupportedException">Only member expression and methodcall expressions are supported.</exception>
        private static string GetPropertyName(Expression expression)
        {
            MemberExpression memberExpression = expression as MemberExpression;

            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            MethodCallExpression methodCallExpression = expression as MethodCallExpression;

            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Name;
            }

            throw new NotSupportedException("Only member expression and methodcall expressions are supported.");
        }
    }
}