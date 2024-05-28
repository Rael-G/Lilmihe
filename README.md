# Little Migration Helper
## Overview
Lilmihe is a versatile database migration tool designed to simplify the process of applying SQL scripts to a database. Whether integrated as a library into your .NET Core project or used as a standalone command-line application, Lilmihe provides a seamless solution for managing database migrations.

## Features
* Automated Script Execution: Automatically detects and executes SQL scripts located in a specified directory.
* Transaction Management: Executes each script within a transaction to ensure atomicity. If an error occurs, changes are rolled back to maintain database integrity.
* Migration Tracking: Keeps track of applied migrations using a dedicated table in the database to avoid reapplying the same script.
* Error Handling: Captures and reports errors encountered during the migration process, including specific commands and files that failed.

## Importing
To use Lilmihe as a library in your .NET project, install the NuGet package:

```bash
dotnet add package Lilmihe.Lib
```

## Building the Tool from the Source Code
### Requirements
* .NET SDK (version 8.0 or later)
* Git
### Steps

* Clone the Repository:
```bash
git clone https://github.com/Rael-G/Lilmihe.git
cd Lilmihe
```

* Publish the Executable:
```bash
dotnet publish src/Lilmihe.Tool/Lilmihe.Tool.csproj
```
The compiled executable will be located in the artifacts/publish/Lilmihe.Tool/release/ directory. You can then use this executable to run the Tool application.

## Usage
### As a Library
Integrate Lilmihe into your .NET Core project to automate database migrations:
```C#
using Lilmihe;
using Microsoft.Data.Sqlite;

var scriptsPath = "path_to_your_scripts";
using var connection = new SqliteConnection("Data Source=database.db");
var migrator = new MigrationHelper(scriptsPath, connection);
var result = await migrator.Migrate();

if (result.Success)
{
    Console.WriteLine("Migration completed successfully.");
    
}
else
{
    Console.WriteLine($"Migration failed: {result.Message}");
    if (result.Error != null)
    {
        Console.WriteLine($"File: {result.Error.Message}");
    }
}
```

### As a Command-Line Application
Use Lilmihe from the command line to execute database migrations:

```bash
lilmihe --sqlite -c "Your_Connection_String" -p "path_to_your_sql_scripts"
```

## Migration Scripts
Place your SQL migration scripts in the specified directory. Scripts should be named in a way that reflects the order they should be applied (e.g., Migration_001_create_table.sql, Migration_002_add_column.sql, Seed_001_populate).
