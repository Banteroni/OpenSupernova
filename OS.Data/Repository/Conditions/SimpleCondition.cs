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
    public SimpleCondition(string field, Operator op, string value, string? model = null)
    {
        Field = field;
        Operator = op;
        Value = value;
        Model = model;
    }

    public SimpleCondition(string field, Operator op, int value, string? model = null)
    {
        Field = field;
        Operator = op;
        Value = value;
        Model = model;
    }

    public SimpleCondition(string field, Operator op, Guid value, string? model = null)
    {
        Field = field;
        Operator = op;
        Value = value;
        Model = model;
    }

    public string Field;
    public string? Model;
    public Operator Operator { get; set; }
    public object Value;
}