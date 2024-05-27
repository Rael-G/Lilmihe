using System.CommandLine;
using Lilmihe;
using Microsoft.Extensions.Logging;

public partial class Program
{
    public static async Task<int> Main(string[] args)
    {
        var connectionString = new Option<string>(
            ["--connection-string", "-c"],
            "The connection string to the database");
        var scriptsPath = new Option<string>(
            ["--path", "-p"],
            "The path to the SQL scripts");
        var sqlite = new Option<bool>(
            [ "--sqlite"] ,
            "Specify SQLite as the DBMS");

        var rootCommand = new RootCommand("Little database migration helper")
        {
            connectionString,
            scriptsPath,
            sqlite
        };

        rootCommand.SetHandler(Run, connectionString, scriptsPath, sqlite);

        return await rootCommand.InvokeAsync(args);
    }

    public static async Task<int> Run(string connectionString, string scriptsPath, bool sqlite)
    {
        var dbms = -1;
        if (sqlite) dbms = MigratorInjector.Sqlite;

        if ( dbms < 0 || string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(scriptsPath))
        {
            Console.WriteLine("Missing required arguments. Please provide a DBMS option, --connection-string, and --path.");
            return -1;
        }

        var migrator = MigratorInjector.Inject(dbms, connectionString, scriptsPath);
        return await RunMigration(migrator);
    }

    public static async Task<int> RunMigration(MigrationHelper migrator)
    {
        Console.WriteLine(Messages.TITLE);

        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Lilmihe");
        factory.Dispose();

        var result = await migrator.Migrate();

        var successMessage = string.Empty;
        var failMessage = string.Empty;
        if (result.Success || result.SuccessFiles.Count > 0)
            successMessage += Messages.MIGRATION_SUCCESS;

        if (result.SuccessFiles.Count > 0)
        {
            successMessage += "\nScripts:";
            foreach (var file in result.SuccessFiles)
            {
                successMessage += "\n" + file;
            }
        }
        else if (result.Success)
        {
            successMessage += "\n" + result.Message;
        }

        if (!string.IsNullOrEmpty(successMessage))
            logger.LogInformation(successMessage);

        if(!result.Success)
        {
            failMessage += Messages.MIGRATION_ERROR;
            failMessage += !string.IsNullOrEmpty(result.Message)? "\n" + result.Message : string.Empty;
            failMessage += (result.FailedFile != null)? "\nFailed File: " + result.FailedFile : string.Empty;
            failMessage += (result.FailedCommand != null)? "\nFailed Command: " + result.FailedCommand : string.Empty;
            failMessage += (result.Error != null)? "\nError: " + result.Error.Message : string.Empty;
        }

        if (!string.IsNullOrEmpty(failMessage))
        {
            logger.LogError(failMessage);
            return -1;
        }

        return 0;
    }
}