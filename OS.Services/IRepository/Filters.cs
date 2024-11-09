namespace OS.Services.IRepository;

public enum Operator
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith,
    IsNull,
}

public class Filters(string field, Operator @operator, string value, bool isNegativeStatement = false)
{
    public required string Field = field;
    public required bool IsNegativeStatement = isNegativeStatement;
    public required Operator Operator = @operator;
    public required string Value = value;
}