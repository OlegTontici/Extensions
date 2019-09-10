using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Extensions.IQueryable.Filtering
{
    public abstract class FilteringOperator
    {
        public int Value { get; }
        public string DisplayName { get; }

        protected FilteringOperator(int value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }

        public static FilteringOperator Equal => new Equal();
        public static FilteringOperator NotEqual => new NotEqual();
        public static FilteringOperator LessThan => new LessThan();
        public static FilteringOperator LessThanOrEqual => new LessThanOrEqual();
        public static FilteringOperator GreaterThan => new GreaterThan();
        public static FilteringOperator GreaterThanOrEqual => new GreaterThanOrEqual();
        public static FilteringOperator Contains => new Contains();
        public static FilteringOperator StartsWith => new StartsWith();
        public static FilteringOperator EndsWith => new EndsWith();

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
            { EndsWith.Value, EndsWith },
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

            if (result == null)
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

        public Expression ToExpression(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            if (!CanBeAppliedTo(memberAccessExpression.Type))
            {
                throw new ArgumentException($"{DisplayName} oprator can not be applied on type {memberAccessExpression.Type}");
            }

            var result = Apply(memberAccessExpression, searchValueExpression);

            return result;
        }

        protected abstract Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression);
        protected abstract bool CanBeAppliedTo(Type type);
    }

    public class Equal : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(DateTime),
            typeof(Guid),
            typeof(bool)
        };

        public Equal() : base(0, "==")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            var result = Expression.Equal(memberAccessExpression, searchValueExpression);

            return result;
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class NotEqual : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(DateTime),
            typeof(Guid),
            typeof(bool)
        };

        public NotEqual() : base(1, "!=")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            var result = Expression.NotEqual(memberAccessExpression, searchValueExpression);

            return result;
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class LessThan : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(DateTime)
        };

        public LessThan() : base(2, "<")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            var result = Expression.LessThan(memberAccessExpression, searchValueExpression);

            return result;
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class LessThanOrEqual : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(DateTime)
        };

        public LessThanOrEqual() : base(3, "<=")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            var result = Expression.LessThanOrEqual(memberAccessExpression, searchValueExpression);

            return result;
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class GreaterThan : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(DateTime)
        };

        public GreaterThan() : base(4, ">")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            var result = Expression.GreaterThan(memberAccessExpression, searchValueExpression);

            return result;
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class GreaterThanOrEqual : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(DateTime)
        };

        public GreaterThanOrEqual() : base(5, ">=")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            var result = Expression.GreaterThanOrEqual(memberAccessExpression, searchValueExpression);

            return result;
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class Contains : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(string)
        };

        public Contains() : base(6, "Contains")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            if (searchValueExpression.Value == null)
            {
                throw new ArgumentNullException("Search value", $"'Contains' filtering does not support null arguments");
            }

            var coalesceExpression = Expression.Coalesce(memberAccessExpression, Expression.Constant(string.Empty));
            return Expression.Call(coalesceExpression, typeof(string).GetMethod("Contains"), searchValueExpression);
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class StartsWith : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(string)
        };

        public StartsWith() : base(7, "StartsWith")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            if (searchValueExpression.Value == null)
            {
                throw new ArgumentNullException("Search value", $"'StartsWith' filtering does not support null arguments");
            }

            var coalesceExpression = Expression.Coalesce(memberAccessExpression, Expression.Constant(string.Empty));
            return Expression.Call(coalesceExpression, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), searchValueExpression);
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
    public class EndsWith : FilteringOperator
    {
        private readonly List<Type> supportedTypes = new List<Type>
        {
            typeof(string)
        };

        public EndsWith() : base(8, "EndsWith")
        {

        }

        protected override Expression Apply(MemberExpression memberAccessExpression, ConstantExpression searchValueExpression)
        {
            if (searchValueExpression.Value == null)
            {
                throw new ArgumentNullException("Search value", $"'EndsWith' filtering does not support null arguments");
            }

            var coalesceExpression = Expression.Coalesce(memberAccessExpression, Expression.Constant(string.Empty));
            return Expression.Call(coalesceExpression, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), searchValueExpression);
        }

        protected override bool CanBeAppliedTo(Type type)
        {
            var canBeApplied = supportedTypes.Any(t => t == type);

            return canBeApplied;
        }
    }
}
