using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.IQueryable.Filtering
{
    public class FilteringOperator
    {
        public int Value { get; }
        public string DisplayName { get; }

        private FilteringOperator(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static FilteringOperator Equal => new FilteringOperator(0, "==");
        public static FilteringOperator NotEqual => new FilteringOperator(1, "!=");
        public static FilteringOperator LessThan => new FilteringOperator(2, "<");
        public static FilteringOperator LessThanOrEqual => new FilteringOperator(3, "<=");
        public static FilteringOperator GreaterThan => new FilteringOperator(4, ">");
        public static FilteringOperator GreaterThanOrEqual => new FilteringOperator(5, ">=");
        public static FilteringOperator Contains => new FilteringOperator(6, "Contains");
        public static FilteringOperator StartsWith => new FilteringOperator(7, "StartsWith");

        private static readonly Dictionary<int, FilteringOperator> Operators = new Dictionary<int, FilteringOperator>
        {
            { Equal.Value, Equal },
            { NotEqual.Value, NotEqual },
            { LessThan.Value, LessThan },
            { LessThanOrEqual.Value, LessThanOrEqual },
            { GreaterThan.Value, GreaterThan },
            { GreaterThanOrEqual.Value, GreaterThanOrEqual },
            { Contains.Value, Contains },
            { StartsWith.Value, StartsWith },
        };

        public static FilteringOperator FromValue(int operatorValue)
        {
            if (!Operators.ContainsKey(operatorValue))
            {
                throw new ArgumentException($"Filtering operator {operatorValue} is not supported");
            }
            var result = Operators[operatorValue];

            return result;
        }

        public static FilteringOperator FromDisplayName(string displayName)
        {
            var result = Operators.Values.FirstOrDefault(o => o.DisplayName == displayName);

            if(result == null)
            {
                throw new ArgumentException($"Filtering operator {displayName} is not supported");
            }

            return result;
        }

        public static IEnumerable<FilteringOperator> GetAll()
        {
            return Operators.Values;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public bool Equals(FilteringOperator obj)
        {
            return obj != null &&
                obj.DisplayName == DisplayName &&
                obj.Value == Value;
        }

        public static implicit operator string(FilteringOperator filteringOperator)
        {
            return filteringOperator.DisplayName;
        }
    }
}
