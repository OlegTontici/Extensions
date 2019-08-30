using System;
namespace Extensions.IQueryable.Filtering
{
    public class Filter
    {
        public string PropertyName { get; }
        public FilteringOperator Operator { get; }
        public object SearchValue { get; }

        public Filter(string propertyName, FilteringOperator filteringOperator, object searchValue)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"{propertyName} can not be empty", nameof(propertyName));
            }

            if(filteringOperator == null)
            {
                throw new ArgumentNullException(nameof(filteringOperator));
            }

            PropertyName = propertyName;
            Operator = filteringOperator;
            SearchValue = searchValue;
        }
    }
}
