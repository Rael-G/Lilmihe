using Microsoft.Extensions.Logging;
using DbUp.Engine;

namespace DbUpInBox;

public abstract class DbMigrator(ILogger logger, string connectionString)
{
    public string ConnectionString { get; set; } = connectionString;
    public ILogger _logger { get; set; } = logger;

    private const string MIGRATION_SUCCESS = "Migration Success.";
    private const string SEED_SUCCESS = "Seed Success.";

    public void Migrate(string scriptsPath)
    {
        var result = ExecuteMigration(ConnectionString, scriptsPath);

        if (result.Successful)
            _logger.LogInformation(MIGRATION_SUCCESS);
        else
            throw new DbException($"DB exception has occurred in script '{result.ErrorScript.Name}'\n{result.Error}");
    }

    public void Seed(string seedPath)
    {
        var result = ExecuteMigration(ConnectionString, seedPath);

        if (result.Successful)
            _logger.LogInformation(SEED_SUCCESS);
        else
            throw new DbException($"DB exception has occurred in script '{result.ErrorScript.Name}'\n{result.Error}");

    }

    protected abstract DatabaseUpgradeResult ExecuteMigration
        (string connectionString, string scriptsPath);
}