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
        public static PaginationResult<T> Paginated<T>(this IQueryable<T> source, PaginationInfo paginationInfo)
        {
            // not async (
            var data = source.Skip((paginationInfo.CurrentPage - 1) * paginationInfo.PageSize).Take(paginationInfo.PageSize).ToList();

            var paginatedResult = new PaginationResult<T>(data, source.Count(), paginationInfo.PageSize, paginationInfo.CurrentPage);

            return paginatedResult;
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
                var filteringExpression = GetFilteringExpression(propertyType);
                return Expression.AndAlso(currentExpression, filteringExpression(parameterExpression, filter));
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

        private static Func<ParameterExpression, Filter, Expression> GetFilteringExpression(Type type)
        {
            return (parameterExpression, filter) =>
            {
                var memberAccessExpression = Expression.Property(parameterExpression, filter.PropertyName);
                var searchValueExpression = Expression.Constant(filter.SearchValue);

                var typeFilteringExpressions = FilteringExpressions[type];
                var filteringExpression = typeFilteringExpressions[filter.Operator];

                if (filteringExpression == null)
                {
                    throw new ArgumentException($"Operator '{filter.Operator}' can not be applied to the type {type.FullName}");
                }

                return filteringExpression(memberAccessExpression, searchValueExpression);
            };
        }

        private static readonly Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>> StringFilteringExpressions =
            new Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>>
            {
                { FilteringOperator.Equal, (memberAccessExpression, searchValueExpression) => Expression.Equal(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.NotEqual, (memberAccessExpression, searchValueExpression) => Expression.NotEqual(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.Contains, (memberAccessExpression, searchValueExpression) =>
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("Contains"), searchValueToStringExpression);
                    }
                },
                { FilteringOperator.StartsWith, (memberAccessExpression, searchValueExpression) =>
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("StartsWith"), searchValueToStringExpression);
                    }
                },
            };

        private static readonly Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>> NumbersFilteringExpressions =
            new Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>>
            {
                { FilteringOperator.Equal, (memberAccessExpression, searchValueExpression) => Expression.Equal(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.NotEqual, (memberAccessExpression, searchValueExpression) => Expression.NotEqual(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.LessThan, (memberAccessExpression, searchValueExpression) => Expression.LessThan(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.LessThanOrEqual, (memberAccessExpression, searchValueExpression) => Expression.LessThanOrEqual(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.GreaterThan, (memberAccessExpression, searchValueExpression) => Expression.GreaterThan(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.GreaterThanOrEqual, (memberAccessExpression, searchValueExpression) => Expression.GreaterThanOrEqual(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.Contains, (memberAccessExpression, searchValueExpression) =>
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("Contains"), searchValueToStringExpression);
                    }
                },
                { FilteringOperator.StartsWith, (memberAccessExpression, searchValueExpression) =>
                    {
                        var propertyToStringExpression = Expression.Call(memberAccessExpression, typeof(object).GetMethod("ToString"));
                        var searchValueToStringExpression = Expression.Call(searchValueExpression, typeof(object).GetMethod("ToString"));
                        return Expression.Call(propertyToStringExpression, typeof(string).GetMethod("StartsWith"), searchValueToStringExpression);
                    }
                },
            };

        private static readonly Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>> DatesFilteringExpressions =
            new Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>>
            {
                { FilteringOperator.Equal, (memberAccessExpression, searchValueExpression) => Expression.Equal(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.NotEqual, (memberAccessExpression, searchValueExpression) => Expression.NotEqual(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.LessThan, (memberAccessExpression, searchValueExpression) => Expression.LessThan(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.LessThanOrEqual, (memberAccessExpression, searchValueExpression) => Expression.LessThanOrEqual(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.GreaterThan, (memberAccessExpression, searchValueExpression) => Expression.GreaterThan(memberAccessExpression, searchValueExpression) },
                { FilteringOperator.GreaterThanOrEqual, (memberAccessExpression, searchValueExpression) => Expression.GreaterThanOrEqual(memberAccessExpression, searchValueExpression) },                
            };

        private static readonly Dictionary<Type, Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>>> FilteringExpressions =
            new Dictionary<Type, Dictionary<string, Func<MemberExpression, ConstantExpression, Expression>>>
            {
                { typeof(string), StringFilteringExpressions },
                { typeof(int), NumbersFilteringExpressions },
                { typeof(decimal), NumbersFilteringExpressions },
                { typeof(double), NumbersFilteringExpressions },
                { typeof(DateTime), DatesFilteringExpressions}
            };
    }
}
