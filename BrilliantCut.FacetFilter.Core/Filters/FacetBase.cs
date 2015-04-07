using System;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find.Api.Querying;
using BrilliantCut.FacetFilter.Core.Filters;

namespace BrilliantCut.FacetFilter.Core.Filters
{
    public abstract class FacetBase<T, TValue> : FilterContentBase<T, TValue>
           where T : IContent
    {
        private Expression<Func<T, object>> _propertyValuesExpressionObject;
        private string _name;

        public Expression<Func<T, TValue>> PropertyValuesExpression { get; set; }

        public Expression<Func<T, object>> PropertyValuesExpressionObject
        {
            get
            {
                if (_propertyValuesExpressionObject == null)
                {
                    var converted = Expression.Convert(PropertyValuesExpression.Body, typeof(object));
                    _propertyValuesExpressionObject = Expression.Lambda<Func<T, object>>(converted, PropertyValuesExpression.Parameters);
                }

                return _propertyValuesExpressionObject;
            }
        }

        public override string Name
        {
            get
            {
                if (_name == null)
                {
                     _name = GetPropertyName(PropertyValuesExpression.Body);
                }

                return _name;
            }
        }
        
        private static string GetPropertyName(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return methodCallExpression.Method.Name;
            }

            throw new NotSupportedException("Only memberexpression and methodcallexpressions are supported.");
        }
    }
}
