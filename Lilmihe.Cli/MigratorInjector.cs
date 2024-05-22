using Microsoft.Extensions.Logging;

namespace Lilmihe;

public static class MigratorInjector
{
    public static IDbMigrator Inject(string connectionString, string dbms, ILogger logger)
    { 
        return dbms switch
        {
            _ => throw new ArgumentException
                ($@"{dbms} is an invalid DBMS option. Please provide a valid DataBase Management System name as an argument."),
        };
    }
}