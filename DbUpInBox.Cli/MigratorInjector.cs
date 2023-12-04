using Microsoft.Extensions.Logging;

namespace DbUpInBox;

public static class MigratorInjector
{
    public static DbMigrator Inject(string connectionString, string dbms, ILogger logger)
    { 
        return dbms switch
        {
            "postgresql" => new PostgresqlMigrator(logger, connectionString),
            _ => throw new ArgumentException
                (@"DBMS is invalid. Please provide a valid DataBase Management System name as an argument."),
        };
    }
}