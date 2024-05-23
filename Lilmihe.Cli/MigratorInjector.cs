using Microsoft.Data.Sqlite;

namespace Lilmihe;

public static class MigratorInjector
{
    public static MigrationHelper Inject(string dbms, string connectionString, string scriptsPath)
    { 
        return dbms switch
        {
            "sqlite" => new MigrationHelper(scriptsPath, new SqliteConnection(connectionString)),
            _ => throw new ArgumentException
                ($"{dbms} is an invalid DBMS option. Please provide a valid DataBase Management System name as an argument."),
        };
    }
}