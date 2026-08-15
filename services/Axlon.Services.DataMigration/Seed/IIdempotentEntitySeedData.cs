using SqlSugar;

namespace Axlon.Services.DataMigration.Seed;

public interface IEntitySeedData<out T> where T : class, new()
{
    IEnumerable<T> InitSeedData();
    IEnumerable<T> SeedData();
    Task CustomizeSeedData(ISqlSugarClient db);
}

public interface IIdempotentEntitySeedData<out T> : IEntitySeedData<T> where T : class, new()
{
    IReadOnlyList<string> KeyColumns { get; }
}

public sealed class DataMigrationAssemblyMarker;
