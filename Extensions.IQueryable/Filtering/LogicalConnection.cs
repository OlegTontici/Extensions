using System.Linq.Expressions;

namespace Extensions.IQueryable.Filtering
{
    public abstract class LogicalConnection
    {
        public abstract Expression Connect(Expression expression1, Expression expression2);

        public static LogicalConnection Or => new LogicalConnectionOr();
        public static LogicalConnection And => new LogicalConnectionAnd();
    }

    public class LogicalConnectionOr : LogicalConnection
    {
        public override Expression Connect(Expression expression1, Expression expression2)
        {
            var result = Expression.OrElse(expression1, expression2);

            return result;
        }
    }

    public class LogicalConnectionAnd : LogicalConnection
    {
        public override Expression Connect(Expression expression1, Expression expression2)
        {
            var result = Expression.AndAlso(expression1, expression2);

            return result;
        }
    }
}
