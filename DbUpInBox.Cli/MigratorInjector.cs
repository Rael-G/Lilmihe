using Microsoft.Extensions.Logging;

namespace DbUpInBox;

public static class MigratorInjector
{
    public static IDbMigrator Inject(string connectionString, string dbms, ILogger logger)
    { 
        return dbms switch
        {
            "postgresql" => new PostgresqlMigrator(logger, connectionString),
            "sqlserver" => new SqlServerMigrator(logger, connectionString),
            _ => throw new ArgumentException
                ($@"{dbms} is an invalid DBMS option. Please provide a valid DataBase Management System name as an argument."),
        };
    }
}