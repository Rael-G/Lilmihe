# Little Migration Helper
## Overview
The Migration Helper is a utility designed to assist in managing and executing database migration scripts written in SQL. This tool is especially useful for applying a series of SQL scripts to a database in a specific order, ensuring that all necessary migrations are applied only once and that the database is kept up to date.

## Features
* Automated Script Execution: Automatically detects and executes SQL scripts located in a specified directory.
* Transaction Management: Executes each script within a transaction to ensure atomicity. If an error occurs, changes are rolled back to maintain database integrity.
* Migration Tracking: Keeps track of applied migrations using a dedicated table in the database to avoid reapplying the same script.
* Error Handling: Captures and reports errors encountered during the migration process, including specific commands and files that failed.
* Customizable: Allows customization of the migration process through virtual methods that can be overridden in derived classes.
