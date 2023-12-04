using DbUpInBox;
using Microsoft.Extensions.Logging;

var connectionString = args.FirstOrDefault();
var dbms = args.ElementAtOrDefault(1);

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger("DbUpInBox");

DbMigrator migrator;

try
{
    migrator = MigratorInjector.Inject(connectionString, dbms, logger);
}
catch(InputException ex)
{
    logger.LogError(ex, ex.Message);
    return -1;
}

var scriptsPath =  Path.Combine(".", "Scripts");
migrator.Migrate(scriptsPath);
 
var isDevelopmentEnvironment = string.Equals(Environment.GetEnvironmentVariable
    ("ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase);

if (!isDevelopmentEnvironment)
    return 0;

var seedPath = Path.Combine(scriptsPath, "Seeds");
migrator.Seed(seedPath);

return 0;