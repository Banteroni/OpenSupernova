namespace OS.Services.IRepository;

public class Query
{
    public required string PageSize;
    public required string PageNumber;
    public required string SortBy;
    public required string SortOrder;
    public required List<Filters> Filters = [];
}