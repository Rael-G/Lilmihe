using DbUp;
using Microsoft.Extensions.Logging;

namespace DbUpInBox;

public class PostgresqlMigrator(ILogger logger, string connectionString) : DbMigratorBase(logger, connectionString)
{
    protected override DbUp.Engine.DatabaseUpgradeResult ExecuteMigration
        (string connectionString, string scriptsPath)
    {
        try
        {
            EnsureDatabase.For.PostgresqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsFromFileSystem(scriptsPath)
                .LogToNowhere()
                .Build();

            return upgrader.PerformUpgrade();
        }
        catch(Npgsql.PostgresException ex)
        {
           throw new DbException(ex.Message);
        }
        
    }
}