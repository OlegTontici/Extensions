using Extensions.IQueryable.Filtering;
using Extensions.IQueryable.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions.IQueryable
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginated<T>(this IQueryable<T> source, PaginationInfo paginationInfo)
        {
            return source.Skip((paginationInfo.CurrentPage - 1) * paginationInfo.PageSize).Take(paginationInfo.PageSize);
        }

        public static IQueryable<T> FilterBy<T>(this IQueryable<T> source, params Filter[] filters)
        {
            if (!filters.Any())
            {
                return source;
            }

            foreach (var filter in filters)
            {
                var propertyMetadata = typeof(T).GetProperty(filter.PropertyName);

                if (propertyMetadata == null)
                {
                    throw new ArgumentException($"Property {filter.PropertyName} does not exist on type {typeof(T).FullName}");
                }
            }

            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");

            var whereExpressionBody = filters.Aggregate<Filter, Expression>(Expression.Constant(true), (currentExpression, filter) =>
            {
                var propertyType = typeof(T).GetProperty(filter.PropertyName).PropertyType;
                var filterCompositionStrategy = GetFilterCompositionStrategry(propertyType);
                return Expression.AndAlso(currentExpression, filterCompositionStrategy(parameterExpression, filter));
            });

            MethodCallExpression whereMethodCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { source.ElementType },
                source.Expression,
                Expression.Lambda(whereExpressionBody, parameterExpression));

            var result = source.Provider.CreateQuery<T>(whereMethodCallExpression);

            return result;
        }

        private static CompositionStrategyDelegate GetFilterCompositionStrategry(Type type)
        {
            return FilterCompositionStrategies[type];
        }

        private delegate Expression CompositionStrategyDelegate(ParameterExpression parameterExpression, Filter filter);
        private static Expression NumbersFilterCompositionStrategry(ParameterExpression parameterExpression, Filter filter)
        {
            var memberAccessExpression = Expression.Property(parameterExpression, filter.PropertyName);
            var searchValueExpression = Expression.Constant(filter.SearchValue);

            switch (filter.Operator)
            {
                case FilteringOperators.Equal: return Expression.Equal(memberAccessExpression, searchValueExpression);
                case FilteringOperators.NotEqual: return Expression.NotEqual(memberAccessExpression, searchValueExpression);
                case FilteringOperators.LessThan: return Expression.LessThan(memberAccessExpression, searchValueExpression);
                case FilteringOperators.LessThanOrEqual: return Expression.LessThanOrEqual(memberAccessExpression, searchValueExpression);
                case FilteringOperators.GreaterThan: return Expression.GreaterThan(memberAccessExpression, searchValueExpression);
                case FilteringOperators.GreaterThanOrEqual: return Expression.GreaterThanOrEqual(memberAccessExpression, searchValueExpression);
                case FilteringOperators.Contains:
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("Contains"), searchValueToStringExpression);
                    }
                case FilteringOperators.StartsWith:
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("StartsWith"), searchValueToStringExpression);
                    }
                default: throw new ArgumentException($"Operator '{filter.Operator}' can not be applied to the type {filter.SearchValue.GetType().FullName}");
            }
        }
        private static Expression StringsFilterCompositionStrategry(ParameterExpression parameterExpression, Filter filter)
        {
            var memberAccessExpression = Expression.Property(parameterExpression, filter.PropertyName);
            var searchValueExpression = Expression.Constant(filter.SearchValue);

            switch (filter.Operator)
            {
                case FilteringOperators.Equal: return Expression.Equal(memberAccessExpression, searchValueExpression);
                case FilteringOperators.NotEqual: return Expression.NotEqual(memberAccessExpression, searchValueExpression);
                case FilteringOperators.Contains:
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("Contains"), searchValueToStringExpression);
                    }
                case FilteringOperators.StartsWith:
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("StartsWith"), searchValueToStringExpression);
                    }
                default: throw new ArgumentException($"Operator '{filter.Operator}' can not be applied to the type {filter.SearchValue.GetType().FullName}");
            }
        }

        private static readonly Dictionary<Type, CompositionStrategyDelegate> FilterCompositionStrategies =
            new Dictionary<Type, CompositionStrategyDelegate>
            {
                { typeof(string) , StringsFilterCompositionStrategry},
                { typeof(int) , NumbersFilterCompositionStrategry},
                { typeof(decimal) , NumbersFilterCompositionStrategry},
                { typeof(double) , NumbersFilterCompositionStrategry}
            };       
    }
}
