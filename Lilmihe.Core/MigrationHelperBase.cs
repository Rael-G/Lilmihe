using System.Data;
using System.Data.Common;
using Dapper;

namespace Lilmihe;

public abstract class MigrationHelperBase : IDbMigrator
{
    protected readonly string ConnectionString;

    private readonly string ScriptsPath;

    private readonly MigrationResult Result = new();

    public MigrationHelperBase(string scriptsPath, string connectionString)
    {
        ScriptsPath = scriptsPath;
        ConnectionString = connectionString;
    }

    public async Task<MigrationResult> Migrate()
    {
        using var connection = CreateConnection();

        var files = Directory.GetFiles(ScriptsPath, "*.sql");
        Array.Sort(files);
        if (files.Length < 1)
        {
            Result.Message = $"There is no SQL files on path '{ScriptsPath}'";
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
            if (await HasMigration(connection, files[i]))
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

    protected virtual async Task CreateMigrationsTable(IDbConnection connection)
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Migrations (
                Id Text PRIMARY KEY
            );
        ";
        await connection.ExecuteAsync(sql);
    }

    protected virtual async Task InsertMigration(IDbConnection connection, string id)
    {
        var sql = @"
            INSERT INTO Migrations (Id)
            VALUES(
                @Id
            );
        ";
        await connection.ExecuteAsync(sql, new {Id = id});
    }

    protected virtual async Task<bool> HasMigration(IDbConnection connection, string id)
    {
        var sql = @"
            SELECT Id 
            FROM Migrations
            WHERE Id = @Id;
        ";

        return await connection.QueryFirstOrDefaultAsync(sql, new {Id = id}) is not null;
    }

    protected abstract IDbConnection CreateConnection();
}
