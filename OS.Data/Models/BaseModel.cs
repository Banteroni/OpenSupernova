namespace OS.Data.Models;

public class BaseModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt;
    public DateTime UpdatedAt;
}