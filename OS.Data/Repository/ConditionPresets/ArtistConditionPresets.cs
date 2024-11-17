using OS.Data.Models;
using OS.Data.Repository.Conditions;

namespace OS.Data.Repository.ConditionPresets;

public static  class ArtistConditionPresets
{
    public static SimpleCondition UnknownArtist()
    {
        return new SimpleCondition(nameof(Artist.Id), Operator.Equal, Guid.Parse("00000000-0000-0000-0000-000000000001"));
    }
    public static SimpleCondition ArtistNameSearch(string name)
    {
        return new SimpleCondition(nameof(Artist.Name), Operator.Contains, name);
    }
}