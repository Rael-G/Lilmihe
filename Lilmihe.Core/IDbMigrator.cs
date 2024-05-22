namespace Lilmihe;

public interface IDbMigrator
{
     public Task<MigrationResult> Migrate();
}
