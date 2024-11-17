using OS.Data.Models;
using OS.Data.Repository.Conditions;

namespace OS.Data.Repository.ConditionPresets;

public static class AlbumConditionPresets
{
    public static CompositeCondition AlbumSearch(string name, string? artistName, int? year)
    {
        var compositeCondition = new CompositeCondition(LogicalSwitch.And);

        compositeCondition.AddCondition(new SimpleCondition(nameof(Album.Name), Operator.Contains, name));

        if (year != null)
        {
            compositeCondition.AddCondition(new SimpleCondition(nameof(Album.Year), Operator.Equal, (int)year));
        }

        if (artistName != null)
        {
            compositeCondition.AddCondition(new SimpleCondition(nameof(Artist.Name), Operator.Contains, artistName,
                nameof(Artist)));
        }

        return compositeCondition;
    }
}