using System.Linq.Expressions;

namespace OS.Data.Repository.Conditions;

public enum LogicalSwitch
{
    And,
    Or
}

public class CompositeCondition : BaseCondition
{
    public List<SimpleCondition> Conditions { get; set; } = [];

    public LogicalSwitch Op { get; set; }

    public int Skip { get; set; }
    public int Take { get; set; }


    public CompositeCondition(LogicalSwitch op, params SimpleCondition[] conditions)
    {
        Op = op;
        Conditions.AddRange(conditions);
    }

    public CompositeCondition(LogicalSwitch op)
    {
        Op = op;
    }

    public CompositeCondition(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    public void AddCondition(SimpleCondition condition)
    {
        Conditions.Add(condition);
    }

    public IEnumerable<string> GetModelsToInclude()
    {
        var models = Conditions.Select(c => c.Model).Distinct().ToList();
        models.RemoveAll(m => m == null);
        return models.AsEnumerable()!;
    }

    public Expression<Func<T, bool>> ToLambda<T>()
    {
        var expressions = new List<Expression>();
        var parameter = Expression.Parameter(typeof(T), "e");
        // if there are no conditions, return a lambda that always returns true
        if (Conditions.Count > 0)
        {
            foreach (var column in Conditions)
            {
                // Check if ModelName is set and if so, use it to build the expression
                Expression member = parameter;

                if (column.Model != null)
                {
                    member = Expression.Property(member, column.Model);
                    member = Expression.Property(member, column.Field);
                }
                else
                {
                    member = Expression.Property(member, column.Field);
                }

                var constant = Expression.Constant(column.Value);

                // Build the expression based on the operator
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
        }
        else
        {
            expressions.Add(Expression.Constant(true));
        }

        Expression body;

        if (Op == LogicalSwitch.And)
        {
            body = expressions.Aggregate((accum, expr) => Expression.AndAlso(accum, expr));
        }
        else
        {
            body = expressions.Aggregate((accum, expr) => Expression.OrElse(accum, expr));
        }
         
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}