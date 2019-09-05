using System;
using System.Linq.Expressions;

namespace Extensions.IQueryable.Filtering
{
    public class Filter
    {
        public string PropertyName { get; }
        public FilteringOperator Operator { get; }
        public object SearchValue { get; }
        public LogicalConnection LogicalConnection { get; private set; }

        public Filter(string propertyName, FilteringOperator filteringOperator, object searchValue, LogicalConnection logicalConnection)
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

        public Filter(string propertyName, FilteringOperator filteringOperator, object searchValue) : this(propertyName, filteringOperator, searchValue, LogicalConnection.And)
        {

        }

        public Expression Connect(Expression firstExpression, Expression secondExpression)
        {
            var result = LogicalConnection.Connect(firstExpression, secondExpression);

            return result;
        }
    }
}
