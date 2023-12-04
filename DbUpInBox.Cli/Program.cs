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
    Console.WriteLine("Options are <postgresql>");
    dbms = Console.ReadLine()?? string.Empty;
}

Console.Clear();
Console.WriteLine(Messages.TITLE);

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("DbUpInBox");
factory.Dispose();

DbMigrator migrator;
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
    logger.LogError(ex, Messages.DIRECTORY_ERROR, new[]{ scriptsPath });
    return -1;
}
catch (SocketException ex)
{
    logger.LogError(ex, $"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_ERROR}",  new[] {connectionString});
    return -1;
}
catch (ArgumentException ex)
{
    logger.LogError(ex, $"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_STRING_ERROR}",  new[] {connectionString});
    return -1;
}
catch(DbException ex)
{
    logger.LogError(ex, $"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_STRING_ERROR}", new[] {connectionString});
    return -1;
}

Console.WriteLine("Do you want to run seeds? (Y/N)");
var key = Console.ReadLine();
if (key is null || !key.Equals("Y", StringComparison.CurrentCultureIgnoreCase))
    return 0;

Console.Clear();
Console.WriteLine(Messages.TITLE);

var seedPath = string.Empty;
while (seedPath.IsNullOrEmpty())
{
    Console.WriteLine("Paste the path to seeds:");
    seedPath = Console.ReadLine()?? string.Empty;
}

try
{
    migrator.Seed(seedPath);
}
catch(DirectoryNotFoundException ex)
{
    logger.LogError(ex, Messages.DIRECTORY_ERROR, new[]{ scriptsPath });
    return -1;
}
catch (SocketException ex)
{
    logger.LogError(ex, $"{Messages.SEED_ERROR} {Messages.CONNECTION_ERROR}",  new[] {connectionString});
    return -1;
}
catch (ArgumentException ex)
{
    logger.LogError(ex, $"{Messages.SEED_ERROR} {Messages.CONNECTION_STRING_ERROR}",  new[] {connectionString});
    return -1;
}
catch(DbException ex)
{
    logger.LogError(ex, $"{Messages.SEED_ERROR}");
    return -1;
}

return 0;