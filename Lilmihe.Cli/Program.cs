using Lilmihe;
using Microsoft.Extensions.Logging;

Console.Clear();
Console.WriteLine(Messages.TITLE);
var dbms = string.Empty;
while (string.IsNullOrEmpty(dbms))
{
    Console.WriteLine("Type your DBMS:");
    Console.WriteLine("Options are <postgresql> <sqlserver>");
    dbms = Console.ReadLine()?? string.Empty;
}

Console.Clear();
Console.WriteLine(Messages.TITLE);
var connectionString = string.Empty;
while(string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("Paste your Connection String:");
    connectionString = Console.ReadLine()?? string.Empty;
}

Console.Clear();
Console.WriteLine(Messages.TITLE);
var scriptsPath = string.Empty;
while (string.IsNullOrEmpty(scriptsPath))
{
    Console.WriteLine("Paste the path to scripts:");
    scriptsPath = Console.ReadLine()?? string.Empty;
}

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("DbUpInBox");
factory.Dispose();

MigrationHelper migrator;
try
{
    migrator = MigratorInjector.Inject(dbms, connectionString, scriptsPath);
}
catch(ArgumentException ex)
{
    logger.LogError(ex, Messages.MIGRATION_ERROR);
    return -1;
}

var result = await migrator.Migrate();

var successMessage = string.Empty;
var failMessage = string.Empty;
if (result.Success || result.SuccessFiles.Length > 0)
    successMessage += Messages.MIGRATION_SUCCESS + "\n";

if (result.SuccessFiles.Length > 0)
{
    successMessage += "Scripts:\n";
    foreach (var file in result.SuccessFiles)
    {
        successMessage += file + "\n";
    }
    successMessage += "\n";
}
else if (result.Success)
{
    successMessage += result.Message +="\n";
}

if (!string.IsNullOrEmpty(successMessage))
    logger.LogInformation(successMessage);

if(!result.Success)
{
    failMessage +=  Messages.MIGRATION_ERROR + "\n";
    failMessage += result.Message + "\n";
    failMessage += (result.FailedFile != null)? "Failed File:\n" + result.FailedFile : string.Empty;
    failMessage += (result.FailedCommand != null)? "Failed Command:\n" + result.FailedCommand : string.Empty;
    failMessage += (result.Error != null)? "Error:\n" + result.Error.Message : string.Empty;
}

if (!string.IsNullOrEmpty(failMessage))
{
    logger.LogError(failMessage);
    return -1;
}

return 0;