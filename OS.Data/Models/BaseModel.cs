using System.Text.Json.Serialization;

namespace OS.Data.Models;

public class  BaseModel
{
    public Guid Id { get; init; }
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public DateTime UpdatedAt { get; set; }
}
