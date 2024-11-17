namespace OS.Data.Repository.Conditions;

public enum Operator
{
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Contains,
    DoesNotContain
}

public class SimpleCondition : BaseCondition
{
    public SimpleCondition(string field, Operator op, string value)
    {
        Field = field;
        Operator = op;
        Value = value;
    }

    public SimpleCondition(string field, Operator op, int value)
    {
        Field = field;
        Operator = op;
        Value = value;
    }

    public SimpleCondition(string field, Operator op, Guid value)
    {
        Field = field;
        Operator = op;
        Value = value;
    }

    public string Field;
    public Operator Operator { get; set; }
    public object Value;
}