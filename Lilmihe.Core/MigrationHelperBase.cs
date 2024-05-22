using System.Data;
using System.Data.Common;
using Dapper;

namespace Lilmihe;

public abstract class MigrationHelperBase : IDbMigrator
{
    private readonly string MigrationsPath;

    private readonly string ConnectionString;

    private readonly MigrationResult Result = new();

    public MigrationHelperBase(string migrationsPath, string connectionString)
    {
        MigrationsPath = migrationsPath;
        ConnectionString = connectionString;
    }

    public async Task<MigrationResult> Migrate()
    {
        using var connection = CreateConnection();

        var files = Directory.GetFiles(MigrationsPath, "*.sql");
        Array.Sort(files);
        if (files.Length < 1)
        {
            Result.Message = $"There is no SQL files on path '{MigrationsPath}'";
            return Result;
        }
        Result.SuccessFiles = new string[files.Length];

        try
        {
            connection.Open();
        }
        catch(DbException e)
        {
            Result.Error = e;
            Result.Message = "Error when trying to establish connection";
            return Result;
        }

        await CreateMigrationsTable(connection);

        try
        {
            await ExecuteFiles(connection, files);
        }
        catch(DbException)
        {
            return Result;
        }

        Result.Success = true;
        if (Result.SuccessFiles.Length == 0)
            Result.Message = "Everything is up to date";
        return Result;
    }

    private async Task ExecuteFiles(IDbConnection connection, string[] files)
    {   
        for(int i = 0; i < files.Length; i++)
        {
            var fileName = Path.GetFileName(files[i]);
            if (connection.QueryFirstOrDefault(GetMigration(files[i])) is null)
            {
                using var transaction = connection.BeginTransaction();
                try
                {
                    var commands = ReadCommandsFromFile(files[i]);
                    await ExecuteCommands(connection, commands);
                    await InsertMigration(connection, fileName);
                }
                catch(DbException)
                {
                    transaction.Rollback();
                    Result.FailedFile = fileName;
                    throw;
                }

                Result.SuccessFiles[i] = files[i];
                transaction.Commit();
            }
        }
    }

    private async Task ExecuteCommands(IDbConnection connection, string[] commands)
    {
        foreach(var command in commands)
        {
            try
            {
                await connection.ExecuteAsync(command);
            }
            catch (DbException e)
            {
                Result.FailedCommand = command.Trim();
                Result.Error = e;
                throw;
            }
        }
    }

    private string[] ReadCommandsFromFile(string file)
    {
        var sql = File.ReadAllText(file);

        return sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
    }

    private async Task CreateMigrationsTable(IDbConnection connection)
    {
        await connection.ExecuteAsync(CreateMigrationsTableSQL);
    }

    private async Task InsertMigration(IDbConnection connection, string file)
    {
        await connection.ExecuteAsync(InsertMigrationSQL(file));
    }

    protected abstract string CreateMigrationsTableSQL { get; set; }

    protected abstract IDbConnection CreateConnection();

    protected abstract string GetMigration(string id);

    protected abstract string InsertMigrationSQL(string id);

}
