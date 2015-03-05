using System;
using System.Linq.Expressions;
using EPiServer.Core;
using EPiServer.Find.Api.Querying;

namespace EPiTube.FasetFilter.Core.Filters
{
    public abstract class FasetBase<T, TValue> : FilterContentBase<T, TValue>
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

        //public Expression<Func<T, Filter>> PropertyValuesExpressionFilter { get; set; }

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

        //protected TValue GetPropertyValue(T value)
        //{
        //    if (_propertyValues == null)
        //    {
        //        _propertyValues = PropertyValuesExpression.Compile(); //PropertyValuesExpression.Compile();
        //    }

        //    return _propertyValues(value);
        //}

        //protected Func<T, TValue> PropertyValue
        //{
        //    get
        //    {
        //        if (_propertyValues == null)
        //        {
        //            _propertyValues = PropertyValuesExpression.Compile();
        //        }

        //        return _propertyValues;
        //    }
        //}

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

        //protected Func<T, TValue> PropertyValues
        //{
        //    get
        //    {
        //        if (_propertyValues == null)
        //        {
        //            _propertyValues = PropertyValuesExpression.Compile();
        //        }

        //        return _propertyValues;
        //    }
        //}
    }
}
