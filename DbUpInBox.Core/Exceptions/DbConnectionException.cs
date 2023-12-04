using System.Data.Common;
using System.Diagnostics;

namespace DbUpInBox;

[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(),nq}}")]
public class DbConnectionException(string msg = "") : DbException
{
    public const string DB_CONNECTION_ERROR = "Db Connection Failed.";
    private string _message = msg;
    public new string Message { get => $"{DB_CONNECTION_ERROR} {_message}";}

    private string DebuggerDisplay => ToString();
}