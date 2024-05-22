using System.Diagnostics;

namespace Lilmihe;

[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(),nq}}")]
public class DbException : System.Data.Common.DbException
{
    public const string DB_ERROR = "Db Error.";
    public DbException() 
        : base(DB_ERROR) {}
    public DbException(string msg) 
        : base($"{DB_ERROR} {msg}") {}

    public DbException(string msg, Exception inner) 
        : base($"{DB_ERROR} {msg}", inner) {}

    private string DebuggerDisplay => ToString();
}