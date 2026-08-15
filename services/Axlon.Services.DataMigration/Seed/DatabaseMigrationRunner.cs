using Axlon.Framework.Abstractions;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Axlon.Services.DataMigration.Seed;

/// <summary>
/// Creates schemas on the entity-selected DBS connections and applies idempotent seed definitions.
/// </summary>
public sealed class DatabaseMigrationRunner
{
    private static readonly MethodInfo InsertSeedEntityMethod = typeof(DatabaseMigrationRunner)
        .GetMethod(nameof(InsertSeedEntityAsync), BindingFlags.Static | BindingFlags.NonPublic)!;

    private readonly ISqlSugarClient _db;
    private readonly ILogger _logger;
    private readonly string _defaultConnectionId;
    private readonly IReadOnlyCollection<Type> _effectiveTypes;

    public DatabaseMigrationRunner(ISqlSugarClient db, ILogger logger, params Assembly[] assemblies)
        : this(db, logger, "Main", assemblies)
    {
    }

    public DatabaseMigrationRunner(
        ISqlSugarClient db,
        ILogger logger,
        string defaultConnectionId,
        params Assembly[] assemblies)
    {
        _db = db;
        _logger = logger;
        _defaultConnectionId = NormalizeConnectionId(defaultConnectionId);
        _effectiveTypes = assemblies.Distinct().SelectMany(GetLoadableTypes).Distinct().ToArray();
    }

    public async Task RunAsync(bool initializeSchema, bool seedData)
    {
        if (!initializeSchema && !seedData)
        {
            _logger.LogInformation("Database schema and seed-data initialization are disabled.");
            return;
        }

        var seedTypes = DiscoverSeedTypes(seedData);

        if (initializeSchema)
            InitializeTables(seedTypes);

        if (seedData)
            await ApplySeedDataAsync(seedTypes);
    }

    private List<Type> DiscoverSeedTypes(bool required)
    {
        var seedTypes = _effectiveTypes
            .Where(type => type is { IsClass: true, IsAbstract: false }
                && type.GetInterfaces().Any(IsIdempotentSeedInterface))
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToList();

        if (required && seedTypes.Count == 0)
            throw new InvalidOperationException("No idempotent seed definitions were discovered.");

        return seedTypes;
    }

    private void InitializeTables(IEnumerable<Type> seedTypes)
    {
        var seedEntityTypes = seedTypes.Select(GetSeedInterface)
            .Select(seedInterface => seedInterface.GetGenericArguments()[0]);

        var tableAttributeTypes = _effectiveTypes.Where(type => type is { IsClass: true, IsAbstract: false }
            && !type.IsGenericTypeDefinition
            && type.GetCustomAttribute<SugarTable>() is not null);

        var rootEntityTypes = _effectiveTypes.Where(type => type is { IsClass: true, IsAbstract: false }
            && !type.IsGenericTypeDefinition
            && InheritsFromRootEntityTkey(type));

        var entityTypes = tableAttributeTypes
            .Concat(rootEntityTypes)
            .Concat(seedEntityTypes)
            .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition
                && type.GetConstructor(Type.EmptyTypes) is not null)
            .Distinct()
            .OrderBy(type => type.FullName, StringComparer.Ordinal)
            .ToArray();

        var entitiesByConnection = entityTypes
            .GroupBy(GetConnectionId, StringComparer.OrdinalIgnoreCase)
            .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _logger.LogInformation(
            "Initializing {Count} database tables via Code First across {ConnectionCount} connections.",
            entityTypes.Length,
            entitiesByConnection.Length);

        foreach (var connectionGroup in entitiesByConnection)
        {
            var database = GetConnection(connectionGroup.Key);
            CreateDatabase(database, connectionGroup.Key);

            foreach (var entityType in connectionGroup)
            {
                _logger.LogInformation(
                    "Initializing database table for {EntityType} on connection {ConnectionId}.",
                    entityType.FullName,
                    connectionGroup.Key);
                database.CodeFirst.InitTables(entityType);
            }
        }
    }

    private async Task ApplySeedDataAsync(IEnumerable<Type> seedTypes)
    {
        foreach (var seedType in seedTypes)
        {
            var seedInterface = GetSeedInterface(seedType);
            var entityType = seedInterface.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(seedType)
                ?? throw new InvalidOperationException($"Unable to create seed definition {seedType.FullName}.");
            var keyColumns = GetKeyColumns(seedType, instance);
            var connectionId = GetConnectionId(entityType);
            var database = GetConnection(connectionId);

            _logger.LogInformation(
                "Applying {SeedType} for {EntityType} on connection {ConnectionId}.",
                seedType.Name,
                entityType.Name,
                connectionId);
            await InsertMissingRecordsAsync(database, entityType, keyColumns,
                InvokeSeedMethod(seedType, instance, nameof(IEntitySeedData<object>.InitSeedData)));
            await InsertMissingRecordsAsync(database, entityType, keyColumns,
                InvokeSeedMethod(seedType, instance, nameof(IEntitySeedData<object>.SeedData)));

            var customizeTask = seedType.GetMethod(nameof(IEntitySeedData<object>.CustomizeSeedData))?
                .Invoke(instance, [database]) as Task;
            if (customizeTask is not null)
                await customizeTask;
        }
    }

    private async Task InsertMissingRecordsAsync(
        ISqlSugarClient database,
        Type entityType,
        IReadOnlyList<string> keyColumns,
        object? seedData)
    {
        if (seedData is not IEnumerable records)
            return;

        var inserted = 0;
        var skipped = 0;
        foreach (var entity in records.Cast<object>())
        {
            if (RecordExists(database, entityType, keyColumns, entity))
            {
                skipped++;
                continue;
            }

            var task = (Task)InsertSeedEntityMethod.MakeGenericMethod(entityType).Invoke(null, [database, entity])!;
            await task;
            inserted++;
        }

        _logger.LogInformation("{EntityType}: inserted {Inserted} seed records and skipped {Skipped} existing records.",
            entityType.Name, inserted, skipped);
    }

    private static async Task InsertSeedEntityAsync<TEntity>(ISqlSugarClient database, object entity)
        where TEntity : class, new()
    {
        await database.Insertable((TEntity)entity).ExecuteCommandAsync();
    }

    private static bool RecordExists(
        ISqlSugarClient database,
        Type entityType,
        IReadOnlyList<string> keyColumns,
        object entity)
    {
        var entityInfo = database.EntityMaintenance.GetEntityInfo(entityType);
        var conditions = new List<string>();
        var parameters = new List<SugarParameter>();

        for (var index = 0; index < keyColumns.Count; index++)
        {
            var keyColumn = keyColumns[index];
            var property = entityType.GetProperty(keyColumn)
                ?? throw new InvalidOperationException($"{entityType.Name} does not expose key property {keyColumn}.");
            var column = entityInfo.Columns.SingleOrDefault(item => item.PropertyName == keyColumn)
                ?? throw new InvalidOperationException($"{entityType.Name}.{keyColumn} is not mapped to a database column.");
            var value = property.GetValue(entity)
                ?? throw new InvalidOperationException($"Seed key {entityType.Name}.{keyColumn} cannot be null.");
            var parameterName = $"@seed_key_{index}";

            conditions.Add($"{QuoteIdentifier(column.DbColumnName)} = {parameterName}");
            parameters.Add(new SugarParameter(parameterName, value));
        }

        var tableName = QuoteIdentifier(entityInfo.DbTableName);
        var sql = $"SELECT COUNT(1) FROM {tableName} WHERE {string.Join(" AND ", conditions)}";
        return database.Ado.GetInt(sql, parameters.ToArray()) > 0;
    }

    private string GetConnectionId(Type entityType)
    {
        var configuredConnectionId = entityType.GetCustomAttribute<TenantAttribute>()?.configId?.ToString();
        return string.IsNullOrWhiteSpace(configuredConnectionId)
            ? _defaultConnectionId
            : NormalizeConnectionId(configuredConnectionId);
    }

    private ISqlSugarClient GetConnection(string connectionId)
    {
        var tenant = _db.AsTenant();
        if (!tenant.IsAnyConnection(connectionId))
        {
            throw new InvalidOperationException(
                $"Database connection '{connectionId}' required by the migration was not found in the enabled DBS configuration.");
        }

        return tenant.GetConnectionScope(connectionId);
    }

    private void CreateDatabase(ISqlSugarClient database, string connectionId)
    {
        _logger.LogInformation("Ensuring database exists for connection {ConnectionId}.", connectionId);
        try
        {
            database.DbMaintenance.CreateDatabase();
        }
        catch (NotSupportedException exception)
        {
            _logger.LogWarning(
                exception,
                "Database provider for connection {ConnectionId} does not support automatic database creation; continuing with table migration.",
                connectionId);
        }
    }

    private static string NormalizeConnectionId(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new InvalidOperationException("The default database connection ID cannot be empty.");

        return connectionId.Trim().ToLowerInvariant();
    }

    private static IReadOnlyList<string> GetKeyColumns(Type seedType, object instance)
    {
        var keyColumns = seedType.GetProperty(nameof(IIdempotentEntitySeedData<object>.KeyColumns))?.GetValue(instance)
            as IReadOnlyList<string>;

        if (keyColumns is null || keyColumns.Count == 0)
            throw new InvalidOperationException($"Seed definition {seedType.FullName} must declare at least one key column.");

        return keyColumns;
    }

    private static object? InvokeSeedMethod(Type seedType, object instance, string methodName) =>
        seedType.GetMethod(methodName)?.Invoke(instance, null);

    private static Type GetSeedInterface(Type seedType) => seedType.GetInterfaces()
        .Single(IsIdempotentSeedInterface);

    private static bool IsIdempotentSeedInterface(Type type) => type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(IIdempotentEntitySeedData<>);

    private static bool InheritsFromRootEntityTkey(Type type)
    {
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(RootEntityTkey<>))
                return true;
        }

        return false;
    }

    private static string QuoteIdentifier(string identifier)
    {
        if (!Regex.IsMatch(identifier, "^[A-Za-z0-9_]+$"))
            throw new InvalidOperationException($"Unsupported database identifier: {identifier}");

        return $"`{identifier}`";
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch (ReflectionTypeLoadException exception) { return exception.Types.OfType<Type>(); }
    }
}
