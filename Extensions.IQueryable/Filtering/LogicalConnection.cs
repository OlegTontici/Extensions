using System;
using System.Linq.Expressions;

namespace Extensions.IQueryable.Filtering
{
    public abstract class LogicalConnection
    {
        public abstract Expression Connect(Expression expression1, Expression expression2);

        public static LogicalConnection Or => new LogicalConnectionOr();
        public static LogicalConnection And => new LogicalConnectionAnd();
        public abstract BinaryExpressionShape GetExpression();
        public delegate BinaryExpression BinaryExpressionShape(Expression expression1, Expression expression2);

        public static LogicalConnection From(string logicalConnection)
        {
            switch (logicalConnection)
            {
                case "Or": return Or;
                case "And": return And;
                default:
                    throw new ArgumentException($"Logical connection {logicalConnection} is not supported");
            }
        }
    }

    public class LogicalConnectionOr : LogicalConnection
    {
        public override Expression Connect(Expression expression1, Expression expression2)
        {
            var result = Expression.OrElse(expression1, expression2);

            return result;
        }

        public override BinaryExpressionShape GetExpression()
        {
            return Expression.OrElse;
        }
    }

    public class LogicalConnectionAnd : LogicalConnection
    {
        public override Expression Connect(Expression expression1, Expression expression2)
        {
            var result = Expression.AndAlso(expression1, expression2);

            return result;
        }

        public override BinaryExpressionShape GetExpression()
        {
            return Expression.AndAlso;
        }
    }
}
