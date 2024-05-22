using Microsoft.Extensions.Logging;

namespace Lilmihe;

public static class MigratorInjector
{
    public static IDbMigrator Inject(string dbms, string connectionString, string scriptsPath)
    { 
        return dbms switch
        {
            _ => throw new ArgumentException
                ($"{dbms} is an invalid DBMS option. Please provide a valid DataBase Management System name as an argument."),
        };
    }
}