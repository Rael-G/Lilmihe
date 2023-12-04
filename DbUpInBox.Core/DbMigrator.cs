using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using DbUp.Engine;

namespace DbUpInBox;

public abstract class DbMigrator(ILogger logger, string connectionString)
{
    public string ConnectionString { get; set; } = connectionString;
    private readonly ILogger _logger = logger;

    public void Migrate(string scriptsPath)
    {
        if (!Directory.Exists(scriptsPath))
        {
            _logger.LogError($"{Messages.MIGRATION_ERROR} Directory not found.",  new[] { scriptsPath });
            return;
        }

        DatabaseUpgradeResult result;

        try 
        {
            result = ExecuteMigration(ConnectionString, scriptsPath);
        }
        catch (SocketException ex)
        {
            _logger.LogError(ex, $"{Messages.MIGRATION_ERROR} {Messages.SOCKET_ERROR}",  new[] {ConnectionString});
            return;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, $"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_STRING_ERROR}",  new[] {ConnectionString});
            return;
        }
        catch(DbConnectionException ex)
        {
            _logger.LogError(ex, $"{Messages.MIGRATION_ERROR} {Messages.CONNECTION_STRING_ERROR}", new[] {ConnectionString});
            return;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, Messages.MIGRATION_ERROR);
            return;
        }

        if (result.Successful)
            _logger.LogInformation(Messages.MIGRATION_SUCCESS);
        else
            _logger.LogError(Messages.MIGRATION_ERROR, new[] {result});
    }

    public void Seed(string seedPath)
    {
        if (!Directory.Exists(seedPath))
        {
            _logger.LogError($"{Messages.SEED_ERROR} {Messages.DIRECTORY_ERROR}", new[] { seedPath });
            return;
        }

        DatabaseUpgradeResult result;
        try 
        {
            result = ExecuteMigration(ConnectionString, seedPath);
        }
        catch (SocketException ex)
        {
            _logger.LogError(ex, $"{Messages.SEED_ERROR} {Messages.SOCKET_ERROR}", new[] {ConnectionString});
            return;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, $"{Messages.SEED_ERROR} {Messages.CONNECTION_STRING_ERROR}", new[] {ConnectionString});
            return;
        }
        catch(DbConnectionException ex)
        {
            _logger.LogError(ex, $"{Messages.SEED_ERROR} {Messages.CONNECTION_STRING_ERROR}", new[] {ConnectionString});
            return;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, Messages.SEED_ERROR);
            return;
        }

        if (result.Successful)
            _logger.LogInformation(Messages.SEED_SUCCESS);
        else
            _logger.LogError(Messages.SEED_ERROR, new[] {result});
    }

    protected abstract DatabaseUpgradeResult ExecuteMigration
        (string connectionString, string scriptsPath);

}