using OS.Data.Repository.Conditions;

namespace OS.Services.Repository;

public abstract class BaseRepository
{
    internal static IQueryable<T> ApplySkipAndTake<T>(IQueryable<T> query, CompositeCondition condition)
    {
        if (condition.Skip > 0)
        {
            query = query.Skip(condition.Skip);
        }

        if (condition.Take > 0)
        {
            query = query.Take(condition.Take);
        }

        return query;
    }
}