using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions.IQueryable.Filtering
{
    public interface IFiltersCollection
    {
        FilterCollection Or(Filter filter);
        FilterCollection And(Filter filter);
    }

    public class FilterCollection : IFiltersCollection
    {
        public Filter[] Filters => _filters.ToArray();
        private List<Filter> _filters;

        public FilterCollection(Filter filter1, Filter filter2)
        {
            _filters = new List<Filter>()
            {
                filter1,
                filter2
            };
        }

        public FilterCollection And(Filter filter)
        {
            var lastFilter = _filters.LastOrDefault();
            _filters.Remove(lastFilter);
            _filters.Add(lastFilter.WithLogicalConnection(LogicalConnection.And));
            _filters.Add(filter);
            return this;
        }

        public FilterCollection Or(Filter filter)
        {
            var lastFilter = _filters.LastOrDefault();
            _filters.Remove(lastFilter);
            _filters.Add(lastFilter.WithLogicalConnection(LogicalConnection.Or));
            _filters.Add(filter);
            return this;
        }

        public static implicit operator Filter[] (FilterCollection filterCollection)
        {
            return filterCollection.Filters;
        }
    }

    public abstract class Filter : IFiltersCollection
    {
        public FilterCollection And(Filter filter)
        {
            return new FilterCollection(this.WithLogicalConnection(LogicalConnection.And), filter);
        }

        public FilterCollection Or(Filter filter)
        {
            return new FilterCollection(this.WithLogicalConnection(LogicalConnection.Or), filter);
        }

        public abstract FilteringExpression ToFilteringExpression(ParameterExpression parameterExpression);
        public abstract Filter WithLogicalConnection(LogicalConnection logicalConnection);
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
            var memberAccessInfo = GetMemberAccessInfo(parameterExpression);

            var targetPropertyType = memberAccessInfo.MemberAccessExpression.Type;

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

            var searchValueExpression = Expression.Constant(searchValue);
            var filteringExpression = Operator.ToExpression(memberAccessInfo.MemberAccessExpression, searchValueExpression);            

            if(memberAccessInfo.ChecksAgainsNullExpression != null)
            {
                return Expression.AndAlso(memberAccessInfo.ChecksAgainsNullExpression, filteringExpression);
            }

            return filteringExpression;
        }

        private MemberAccessInfo GetMemberAccessInfo(ParameterExpression parameterExpression)
        {
            var splittedPropertyNames = this.PropertyName.Split('.');           

            var memberAccessExpression = splittedPropertyNames.Aggregate<string, MemberAccessInfo>(null, (currentMemberAccessExpression, currentProperty) => 
            {
                if(currentMemberAccessExpression == null)
                {
                    var typeDoesNotContainTargetProperty = parameterExpression.Type.GetProperty(currentProperty) == null;
                    if (typeDoesNotContainTargetProperty)
                    {
                        throw new ArgumentException($"Property '{PropertyName}' does not exist on the type {parameterExpression.Type.FullName}");
                    }

                    var memberExpression = Expression.Property(parameterExpression, currentProperty);
                    var result = new MemberAccessInfo
                    {
                        MemberAccessExpression = memberExpression
                    };
                    return result;
                }

                var typeDoesNotContainTargetProperty2 = currentMemberAccessExpression.MemberAccessExpression.Type.GetProperty(currentProperty) == null;
                if (typeDoesNotContainTargetProperty2)
                {
                    throw new ArgumentException($"Property '{PropertyName}' does not exist on the type {parameterExpression.Type.FullName}");
                }

                var memberExpression2 = Expression.Property(currentMemberAccessExpression.MemberAccessExpression, currentProperty);
                var checkAgainstNullExpression = currentMemberAccessExpression.ChecksAgainsNullExpression;

                var currentMemberIsNullable = currentMemberAccessExpression.MemberAccessExpression.Type.IsClass
                || (currentMemberAccessExpression.MemberAccessExpression.Type.IsValueType && Nullable.GetUnderlyingType(currentMemberAccessExpression.MemberAccessExpression.Type) != null);

                if (currentMemberIsNullable)
                {
                    var notNullExpression = Expression.NotEqual(currentMemberAccessExpression.MemberAccessExpression, Expression.Constant(null));    
                    if(checkAgainstNullExpression == null)
                    {
                        checkAgainstNullExpression = notNullExpression;
                    }
                    else
                    {
                        checkAgainstNullExpression = Expression.AndAlso(checkAgainstNullExpression, notNullExpression);
                    }
                }

                return new MemberAccessInfo
                {
                    MemberAccessExpression = memberExpression2,
                    ChecksAgainsNullExpression = checkAgainstNullExpression
                };
            });

            return memberAccessExpression;
        }

        public override Filter WithLogicalConnection(LogicalConnection logicalConnection)
        {
            return new SimpleFilter(logicalConnection, PropertyName, Operator, SearchValue);
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

        private class MemberAccessInfo
        {
            public MemberAccessInfo()
            {
                ChecksAgainsNullExpression = null;
            }

            public Expression ChecksAgainsNullExpression { get; set; }
            public MemberExpression MemberAccessExpression { get; set; }
        }
    }

    public class ScopedFilter : Filter
    {
        public LogicalConnection LogicalConnection { get; }
        public IEnumerable<Filter> Filters { get; }

        public ScopedFilter(LogicalConnection logicalConnection, params Filter[] filters)
        {
            LogicalConnection = logicalConnection;
            Filters = filters;
        }

        public ScopedFilter(params Filter[] filters)
        {
            LogicalConnection = LogicalConnection.And;
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

            return filteringExpression.WithLogicalConnection(LogicalConnection.GetExpression());
        }

        public override Filter WithLogicalConnection(LogicalConnection logicalConnection)
        {
            return new ScopedFilter(logicalConnection, Filters.ToArray());
        }
    }
}
