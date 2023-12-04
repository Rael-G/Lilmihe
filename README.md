# DbUpInBox
DbUpInBox is a command-line utility for managing and automating database migrations. The utility utilizes the DbUp library for handling migrations and provides a user-friendly console interface. You can also use its .Core + .Database packages standalone in you program.

Original DbUp description 
>DbUp is a set of .NET libraries that helps you to deploy changes to different databases like SQL Server. It tracks which SQL scripts have been run already, >and runs the change scripts that are needed to get your database up to date.

## Getting Started
* Connection String Input: Users are prompted to input their database connection string, ensuring a secure and customizable setup.  
* DBMS Selection: Choose the desired Database Management System (DBMS) from the available options. Currently, only PostgreSQL is supported.  
* Script Execution: Specify the path to your SQL scripts, allowing DbUpInBox to execute the necessary changes to your database schema.