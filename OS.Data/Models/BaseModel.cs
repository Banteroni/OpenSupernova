namespace OS.Data.Models;

public class BaseModel
{
    public Guid Id { get; init; }
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
}