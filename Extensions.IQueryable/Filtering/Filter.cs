using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions.IQueryable.Filtering
{
    public abstract class Filter
    {
        public abstract FilteringExpression ToFilteringExpression(ParameterExpression parameterExpression);

        public static SimpleFilter Simple(LogicalConnection logicalConnection, string propertyName, FilteringOperator filteringOperator, object searchValue)
        {
            return new SimpleFilter(logicalConnection, propertyName, filteringOperator, searchValue);
        }
        public static SimpleFilter Simple(string propertyName, FilteringOperator filteringOperator, object searchValue)
        {
            return new SimpleFilter(propertyName, filteringOperator, searchValue);
        }
        public static SimpleFilter Simple(string logicalConnection, string propertyName, string filteringOperator, object searchValue)
        {
            return new SimpleFilter(logicalConnection, propertyName, filteringOperator, searchValue);
        }
        public static SimpleFilter Simple(string propertyName, string filteringOperator, object searchValue)
        {
            return new SimpleFilter(propertyName, filteringOperator, searchValue);
        }

        public static ScopedFilter Scoped(LogicalConnection logicalConnection, params SimpleFilter[] filters)
        {
            return new ScopedFilter(logicalConnection, filters);
        }
    }

    public class SimpleFilter : Filter
    {
        public string PropertyName { get; }
        public FilteringOperator Operator { get; }
        public object SearchValue { get; }
        public LogicalConnection LogicalConnection { get; }

        public SimpleFilter(LogicalConnection logicalConnection, string propertyName, FilteringOperator filteringOperator, object searchValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"{propertyName} can not be empty", nameof(propertyName));
            }

            if (filteringOperator == null)
            {
                throw new ArgumentNullException(nameof(filteringOperator));
            }

            if (logicalConnection == null)
            {
                throw new ArgumentNullException(nameof(logicalConnection));
            }

            PropertyName = propertyName;
            Operator = filteringOperator;
            SearchValue = searchValue;
            LogicalConnection = logicalConnection;
        }

        public SimpleFilter(string propertyName, FilteringOperator filteringOperator, object searchValue) : this(LogicalConnection.And, propertyName, filteringOperator, searchValue)
        {

        }

        public SimpleFilter(string logicalConnection, string propertyName, string filteringOperator, object searchValue) :
            this(LogicalConnection.From(logicalConnection), propertyName, FilteringOperator.FromDisplayName(filteringOperator), searchValue)
        {

        }

        public SimpleFilter(string propertyName, string filteringOperator, object searchValue) :
            this(propertyName, FilteringOperator.FromDisplayName(filteringOperator), searchValue)
        {

        }

        public override FilteringExpression ToFilteringExpression(ParameterExpression parameterExpression)
        {
            var expression = GetFilteringExpression(parameterExpression);
            var result = new FilteringExpression(expression, LogicalConnection.GetExpression());

            return result;
        }


        private Expression GetFilteringExpression(ParameterExpression parameterExpression)
        {
            var typeDoesNotContainTargetProperty = parameterExpression.Type.GetProperty(PropertyName) == null;

            if (typeDoesNotContainTargetProperty)
            {
                throw new ArgumentException($"Property '{PropertyName}' does not exist on the type {parameterExpression.Type.FullName}");
            }


            var targetPropertyType = parameterExpression.Type.GetProperty(PropertyName).PropertyType;

            var searchValue = SearchValue;

            var searchValueType = SearchValue?.GetType();

            if (searchValueType != null && searchValueType != targetPropertyType)
            {
                bool searchValueCanBeConverted = TypesSupportedConversion.ContainsKey(searchValueType) && TypesSupportedConversion[searchValueType].Any(t => t == targetPropertyType);

                if (!searchValueCanBeConverted)
                {
                    throw new Exception($"Types missmatch. Property {PropertyName} type of {targetPropertyType} can not be compared against the search value type of {searchValueType}");
                }

                try
                {
                    var convertedSearchValue = Convert.ChangeType(SearchValue, targetPropertyType);
                    searchValue = convertedSearchValue;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Type conversion failed. Value '{searchValue}' can not be converted to the type {targetPropertyType} of the property {PropertyName}");
                }
            }

            var memberAccessExpression = Expression.Property(parameterExpression, PropertyName);
            var searchValueExpression = Expression.Constant(searchValue);

            var filteringExpression = Operator.ToExpression(memberAccessExpression, searchValueExpression);

            return filteringExpression;
        }

        // TODO to add support for string to guid conversion
        private static readonly Dictionary<Type, IEnumerable<Type>> TypesSupportedConversion = new Dictionary<Type, IEnumerable<Type>>
        {
            { typeof(decimal), new List<Type> { typeof(double), typeof(string) } },
            { typeof(double), new List<Type> { typeof(decimal), typeof(string) } },
            { typeof(int), new List<Type> { typeof(double), typeof(decimal), typeof(string) } },
            { typeof(string), new List<Type> { typeof(decimal), typeof(double), typeof(int), typeof(DateTime) } },
            { typeof(DateTime), new List<Type> { typeof(string) } },
            { typeof(Guid), new List<Type> { typeof(string) } }
        };
    }

    public class ScopedFilter : Filter
    {
        public LogicalConnection LogicalConnection { get; }
        public IEnumerable<SimpleFilter> Filters { get; }

        public ScopedFilter(LogicalConnection logicalConnection, params SimpleFilter[] filters)
        {
            LogicalConnection = logicalConnection;
            Filters = filters;
        }

        public override FilteringExpression ToFilteringExpression(ParameterExpression parameterExpression)
        {
            var filteringExpressions = Filters.Select(x => x.ToFilteringExpression(parameterExpression)).ToList();

            FilteringExpression filteringExpression = null;

            foreach (var currentExpression in filteringExpressions)
            {
                if (filteringExpression == null)
                {
                    filteringExpression = currentExpression;
                    continue;
                }

                filteringExpression = filteringExpression.ConnectTo(currentExpression, parameterExpression);
            }

            return filteringExpression;
        }
    }
}
