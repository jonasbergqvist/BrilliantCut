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

    public abstract class FacetBase<T, TValue> : FilterContentBase<T, TValue>
        where T : IContent
    {
        private string _name;

        private Expression<Func<T, object>> _propertyValuesExpressionObject;

        public override string Name
        {
            get
            {
                if (this._name == null)
                {
                    this._name = GetPropertyName(expression: this.PropertyValuesExpression.Body);
                }

                return this._name;
            }
        }

        public Expression<Func<T, TValue>> PropertyValuesExpression { get; set; }

        public Expression<Func<T, object>> PropertyValuesExpressionObject
        {
            get
            {
                if (this._propertyValuesExpressionObject == null)
                {
                    UnaryExpression converted = Expression.Convert(
                        expression: this.PropertyValuesExpression.Body,
                        type: typeof(object));
                    this._propertyValuesExpressionObject = Expression.Lambda<Func<T, object>>(
                        body: converted,
                        parameters: this.PropertyValuesExpression.Parameters);
                }

                return this._propertyValuesExpressionObject;
            }
        }

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

            throw new NotSupportedException("Only memberexpression and methodcallexpressions are supported.");
        }
    }
}