using System.Net.Sockets;
using DbUpInBox;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

Console.Clear();
Console.WriteLine(Messages.TITLE);
var connectionString = string.Empty;
while(connectionString.IsNullOrEmpty())
{
    Console.WriteLine("Paste your Connection String:");
    connectionString = Console.ReadLine()?? string.Empty;
}
Console.Clear();
Console.WriteLine(Messages.TITLE);
var dbms = string.Empty;
while (dbms.IsNullOrEmpty())
{
    Console.WriteLine("Type your DBMS:");
    Console.WriteLine("Options are <postgresql> <sqlserver>");
    dbms = Console.ReadLine()?? string.Empty;
}

Console.Clear();
Console.WriteLine(Messages.TITLE);

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("DbUpInBox");
factory.Dispose();

IDbMigrator migrator;
try
{
    migrator = MigratorInjector.Inject(connectionString, dbms, logger);
}
catch(ArgumentException ex)
{
    logger.LogError(ex, Messages.MIGRATION_ERROR);
    return -1;
}

var scriptsPath = string.Empty;
while (scriptsPath.IsNullOrEmpty())
{
    Console.WriteLine("Paste the path to scripts:");
    scriptsPath = Console.ReadLine()?? string.Empty;
}

try
{
    migrator.Migrate(scriptsPath);
}
catch(DirectoryNotFoundException ex)
{
    logger.LogError(Messages.DIRECTORY_ERROR + ex.Message, new[]{ scriptsPath });
    return -1;
}
catch (SocketException)
{
    logger.LogError($"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_ERROR}",  new[] {connectionString});
    return -1;
}
catch (ArgumentException)
{
    logger.LogError($"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_STRING_ERROR}",  new[] {connectionString});
    return -1;
}
catch(DbException)
{
    logger.LogError($"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_STRING_ERROR}", new[] {connectionString});
    return -1;
}

return 0;