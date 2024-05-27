using Microsoft.Data.Sqlite;

namespace Lilmihe;

public static class MigratorInjector
{
    public const int Sqlite = 0;
    public static MigrationHelper Inject(int dbms, string connectionString, string scriptsPath)
    { 
        return dbms switch
        {
            Sqlite => new MigrationHelper(scriptsPath, new SqliteConnection(connectionString)),
            _ => throw new ArgumentException
                ($"{dbms} is an invalid DBMS option. Please provide a valid DataBase Management System name as an argument."),
        };
    }
}