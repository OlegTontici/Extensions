using System;
namespace Extensions.IQueryable.Filtering
{
    public class Filter
    {
        public string PropertyName { get; }
        public FilteringOperator Operator { get; }
        public object SearchValue { get; }

        public Filter(string propertyName, FilteringOperator @operator, object searchValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"{propertyName} can not be empty");
            }

            PropertyName = propertyName;
            Operator = @operator;
            SearchValue = searchValue;
        }
    }
}
