using DbUp;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DbUpInBox;

public class SqlServerMigrator(ILogger logger, string connectionString) : DbMigratorBase(logger, connectionString)
{
    protected override DbUp.Engine.DatabaseUpgradeResult ExecuteMigration
        (string connectionString, string scriptsPath)
    {
        try
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem(scriptsPath)
                .LogToNowhere()
                .Build();

            return upgrader.PerformUpgrade();
        }
        catch(SqlException ex)
        {
           throw new DbException(ex.Message);
        }
        
    }
}