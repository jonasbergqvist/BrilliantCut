// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Activator.cs" company="Jonas Bergqvist">
//     Copyright © 2019 Jonas Bergqvist.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BrilliantCut.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using EPiServer;
    using EPiServer.Framework.Cache;
    using EPiServer.ServiceLocation;

    /// <summary>
    /// Creates an instance of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to activate</typeparam>
    public class Activator<T>
    {
        private IObjectInstanceCache objectInstanceCache;

        private delegate T ObjectActivator(params object[] args);

        private IObjectInstanceCache ObjectInstanceCache
        {
            get
            {
                return this.objectInstanceCache
                       ?? (this.objectInstanceCache = ServiceLocator.Current.GetInstance<IObjectInstanceCache>());
            }

            set
            {
                this.objectInstanceCache = value;
            }
        }

        /// <summary>
        /// Creates an instance of the <see cref="T"/>.
        /// </summary>
        /// <param name="args">Arguments, used for constructor creation, and for creating a generic type</param>
        /// <returns>The requested <see cref="T"/>.</returns>
        /// <remarks>The supports generic arguments.</remarks>
        /// <exception cref="T:System.Exception">A delegate callback throws an exception.</exception>
        public T Activate(params object[] args)
        {
            return this.Activate(typeof(T), constructorArguments: args);
        }

        /// <summary>
        /// Creates an instance of the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="constructorArguments">Arguments, used for constructor creation.</param>
        /// <returns>The requested <paramref name="type"/>.</returns>
        /// <remarks>The supports generic arguments.</remarks>
        /// <exception cref="T:System.Exception">A delegate callback throws an exception.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="constructorArguments" /> is <see langword="null" />.</exception>
        public T Activate(Type type, params object[] constructorArguments)
        {
            Type[] argumentTypes = constructorArguments.Select(x => x != null ? x.GetOriginalType() : typeof(object))
                .ToArray();

            string cacheKey = string.Concat(
                type.GetHashCode(),
                "#",
                string.Join(":", argumentTypes.Select(x => x.GetHashCode())));

            ObjectActivator objectActivator = this.ObjectInstanceCache.ReadThrough(
                key: cacheKey,
                readValue: () =>
                    {
                        Type finalType = type.IsGenericTypeDefinition
                                             ? type.MakeGenericType(typeArguments: argumentTypes)
                                             : type;
                        return GetObjectActivator(type: finalType, constructorArgumentTypes: argumentTypes);
                    },
                readStrategy: ReadStrategy.Wait);

            return objectActivator(args: constructorArguments);
        }

        /// <summary>
        /// Creates <see cref="ObjectActivator"/>, that will be used to create the requested type.
        /// </summary>
        /// <param name="constructorInfo">The constructor information.</param>
        /// <param name="constructorTypeExpressions">Expressions for the constructor types.</param>
        /// <param name="delegateParameterExpression">The delegate parameter expression.</param>
        /// <returns>The <see cref="ObjectActivator"/></returns>
        private static ObjectActivator CreateDelegate(
            ConstructorInfo constructorInfo,
            IEnumerable<Expression> constructorTypeExpressions,
            ParameterExpression delegateParameterExpression)
        {
            NewExpression constructorExpression = Expression.New(
                constructor: constructorInfo,
                arguments: constructorTypeExpressions);
            LambdaExpression lambdaExpression = Expression.Lambda(
                typeof(ObjectActivator),
                constructorExpression,
                delegateParameterExpression);

            return (ObjectActivator)lambdaExpression.Compile();
        }

        /// <summary>
        /// Creates expressions for the constructor arguments.
        /// </summary>
        /// <param name="constructorArgumentTypes">The constructor argument types.</param>
        /// <param name="delegateParameterExpression">The expression for the delegate parameter.</param>
        /// <returns>Expressions for the constructor arguments.</returns>
        private static IEnumerable<Expression> CreateTypeExpressions(
            Type[] constructorArgumentTypes,
            Expression delegateParameterExpression)
        {
            for (int i = 0; i < constructorArgumentTypes.Length; i++)
            {
                Type paramType = constructorArgumentTypes[i];

                ConstantExpression indexExpression = Expression.Constant(value: i);
                BinaryExpression paramAccessorExpression = Expression.ArrayIndex(
                    array: delegateParameterExpression,
                    index: indexExpression);
                UnaryExpression paramCastExpression = Expression.Convert(
                    expression: paramAccessorExpression,
                    type: paramType);

                yield return paramCastExpression;
            }
        }

        /// <summary>
        /// Gets the objector activator delegate, that will be used to create the requested object.
        /// </summary>
        /// <param name="type">The type to activate</param>
        /// <param name="constructorArgumentTypes">The constructor information.</param>
        /// <returns>The delegate with instructions to create the specified type.</returns>
        private static ObjectActivator GetObjectActivator(Type type, Type[] constructorArgumentTypes)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(types: constructorArgumentTypes);
            ParameterExpression delegateParameterExpression = Expression.Parameter(typeof(object[]), "args");

            List<Expression> typeExpressions = CreateTypeExpressions(
                constructorArgumentTypes: constructorArgumentTypes,
                delegateParameterExpression: delegateParameterExpression).ToList();
            return CreateDelegate(
                constructorInfo: constructorInfo,
                constructorTypeExpressions: typeExpressions,
                delegateParameterExpression: delegateParameterExpression);
        }
    }
}