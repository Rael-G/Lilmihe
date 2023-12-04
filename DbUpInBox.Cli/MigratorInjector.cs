using Microsoft.Extensions.Logging;

namespace DbUpInBox;

public static class MigratorInjector
{
    public static DbMigrator Inject(string? connectionString, string? dbms, ILogger logger)
    {
        if (connectionString is null)
            throw new InputException
                ("Connection string is missing. Please provide a connection string as an argument.");

        if (dbms is null)
               throw new InputException
                ("DBMS is missing. Please provide a your DataBase Management System name as an argument.");
            
        return dbms switch
        {
            "postgresql" => new PostgresqlMigrator(logger, connectionString),
            _ => throw new InputException
                (@"DBMS is invalid. Please provide a valid DataBase Management System nameas an argument. Options are:
                <postgresql>"),
        };
    }
}