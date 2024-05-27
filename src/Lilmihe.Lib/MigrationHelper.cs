using System.Data;
using System.Data.Common;
using Dapper;

namespace Lilmihe;

public class MigrationHelper
{
    protected readonly IDbConnection Connection;

    private readonly string ScriptsPath;

    private readonly MigrationResult Result = new();

    public MigrationHelper(string scriptsPath, IDbConnection connection)
    {
        ScriptsPath = scriptsPath;
        Connection = connection;
    }

    public async Task<MigrationResult> Migrate()
    {
        string[] files;
        try
        {
            files = Directory.GetFiles(ScriptsPath, "*.sql");
        }
        catch(DirectoryNotFoundException e)
        {
            Result.Error = e;
            return Result;
        }

        if (files.Length < 1)
        {
            Result.Message = $"There is no SQL files on path '{ScriptsPath}'";
            return Result;
        }

        Array.Sort(files);

        using (Connection)
        {
            if (Connection.State != ConnectionState.Open)
                try
                {
                    Connection.Open();
                }
                catch(DbException e)
                {
                    Result.Error = e;
                    Result.Message = "Error when trying to establish connection";
                    return Result;
                }

            await CreateMigrationsTable();

            try
            {
                await ExecuteFiles(files);
            }
            catch(DbException)
            {
                return Result;
            }

            Result.Success = true;
            if (Result.SuccessFiles.Count == 0)
                Result.Message = "Everything is up to date";
            return Result;
        }
    }

    private async Task ExecuteFiles(string[] files)
    {   
        foreach(var file in files)
        {
            var fileName = Path.GetFileName(file);
            if (!await HasMigration(fileName))
            {
                using var transaction = Connection.BeginTransaction();
                try
                {
                    var commands = ReadCommandsFromFile(file);
                    await ExecuteCommands(commands);
                    await InsertMigration(fileName);
                }
                catch(DbException)
                {
                    transaction.Rollback();
                    Result.FailedFile = fileName;
                    throw;
                }

                Result.SuccessFiles.Add(fileName);
                transaction.Commit();
            }
        }
    }

    private async Task ExecuteCommands(string[] commands)
    {
        foreach(var command in commands)
        {
            try
            {
                await Connection.ExecuteAsync(command);
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

    protected virtual async Task CreateMigrationsTable()
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Migrations (
                Id Text PRIMARY KEY
            );
        ";
        await Connection.ExecuteAsync(sql);
    }

    protected virtual async Task InsertMigration(string id)
    {
        var sql = @"
            INSERT INTO Migrations (Id)
            VALUES(
                @Id
            );
        ";
        await Connection.ExecuteAsync(sql, new {Id = id});
    }

    protected virtual async Task<bool> HasMigration(string id)
    {
        var sql = @"
            SELECT Id 
            FROM Migrations
            WHERE Id = @Id;
        ";

        return await Connection.QueryFirstOrDefaultAsync(sql, new {Id = id}) is not null;
    }
}
