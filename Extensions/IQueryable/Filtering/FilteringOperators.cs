using System;

namespace Extensions.IQueryable.Filtering
{
    public enum FilteringOperators
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Contains,
        StartsWith
    }

    public static class FilteringOperatorsExtensions
    {
        /// <summary>
        /// <para>Returns a filtering operator based on the operator string representation</para>
        /// <para>Accepted values: ==, !=, &lt;, &lt;=, >, >=, Contains, StartsWith</para>
        /// </summary>
        /// <param name="operators"></param>
        /// <param name="filteringOperator"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Thrown when there are no matching operators for provided value</exception>
        public static FilteringOperators From(this FilteringOperators operators, string filteringOperator)
        {
            switch (filteringOperator)
            {
                case "==": return FilteringOperators.Equal;
                case "!=": return FilteringOperators.NotEqual;
                case "<": return FilteringOperators.LessThan;
                case "<=": return FilteringOperators.LessThanOrEqual;
                case ">": return FilteringOperators.GreaterThan;
                case ">=": return FilteringOperators.GreaterThanOrEqual;
                case "Contains": return FilteringOperators.Contains;
                case "StartsWith": return FilteringOperators.StartsWith;
                default:
                    throw new NotSupportedException($"Operator '{filteringOperator}' is not supported");
            }
        }
    }
}
