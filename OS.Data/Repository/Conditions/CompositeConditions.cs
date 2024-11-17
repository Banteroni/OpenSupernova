using System.Linq.Expressions;

namespace OS.Data.Repository.Conditions;

public enum LogicalSwitch
{
    And,
    Or
}

public class CompositeConditions : BaseCondition
{
    public List<SimpleCondition> Conditions { get; set; } = [];

    public LogicalSwitch Op { get; set; }


    public CompositeConditions(LogicalSwitch op, params SimpleCondition[] conditions)
    {
        Op = op;
        Conditions.AddRange(conditions);
    }

    public CompositeConditions(LogicalSwitch op)
    {
        Op = op;
    }

    public void AddCondition(SimpleCondition condition)
    {
        Conditions.Add(condition);
    }

    public Expression<Func<T, bool>> ToLambda<T>()
    {
        var expressions = new List<Expression>();
        var parameter = Expression.Parameter(typeof(T), "e");

        foreach (var column in Conditions)
        {
            var member = Expression.Property(parameter, column.Field);
            var constant = Expression.Constant(column.Value);
            switch (column.Operator)
            {
                case Operator.Equal:
                    expressions.Add(Expression.Equal(member, constant));
                    break;
                case Operator.NotEqual:
                    expressions.Add(Expression.NotEqual(member, constant));
                    break;
                case Operator.GreaterThan:
                    expressions.Add(Expression.GreaterThan(member, constant));
                    break;
                case Operator.LessThan:
                    expressions.Add(Expression.LessThan(member, constant));
                    break;
                case Operator.GreaterThanOrEqual:
                    expressions.Add(Expression.GreaterThanOrEqual(member, constant));
                    break;
                case Operator.LessThanOrEqual:
                    expressions.Add(Expression.LessThanOrEqual(member, constant));
                    break;
                case Operator.Contains:
                    expressions.Add(Expression.Call(member, "Contains", null, constant));
                    break;
                case Operator.DoesNotContain:
                    expressions.Add(Expression.Not(Expression.Call(member, "Contains", null, constant)));
                    break;
            }
        }

        var resultExpression = expressions.Aggregate(Op == LogicalSwitch.And ? Expression.And : Expression.Or);
        return (Expression<Func<T, bool>>)Expression.Lambda(resultExpression, parameter);
    }
}