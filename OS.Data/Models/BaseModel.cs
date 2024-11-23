namespace OS.Data.Models;

public class  BaseModel
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
