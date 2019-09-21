using Extensions.IQueryable.Filtering;
using Extensions.IQueryable.Pagination;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions.IQueryable
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginated<T>(this IQueryable<T> source, PaginationInfo paginationInfo)
        {
            var paginatedQuery = source.Skip((paginationInfo.CurrentPage - 1) * paginationInfo.PageSize).Take(paginationInfo.PageSize);

            return paginatedQuery;
        }

        public static IQueryable<T> FilterBy<T>(this IQueryable<T> source, params Filter[] filters)
        {
            if (!filters.Any())
            {
                return source;
            }

            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "x");

            var whereExpressionBody = filters.Aggregate<Filter, FilteringExpression>(null, (currentExpression, filter) =>
            {
                if (currentExpression == null)
                {
                    return filter.ToFilteringExpression(parameterExpression);
                }

                var expression = currentExpression.ConnectTo(filter.ToFilteringExpression(parameterExpression));

                return expression;
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
    }
}
