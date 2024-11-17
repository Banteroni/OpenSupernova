using OS.Data.Repository.Conditions;

namespace OS.Data.Repository.SimpleConditions;

public static  class ArtistSimpleConditions
{
    public static SimpleCondition UnknownArtist()
    {
        return new SimpleCondition("Id", Operator.Equal, Guid.Parse("00000000-0000-0000-0000-000000000001"));
    }
    public static SimpleCondition ArtistNameSearch(string name)
    {
        return new SimpleCondition("Name", Operator.Contains, name);
    }
}