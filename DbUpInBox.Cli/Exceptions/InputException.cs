using System.Data.Common;
using System.Diagnostics;

namespace DbUpInBox;

[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(),nq}}")]
public class InputException(string msg = "") : DbException
{
    private const string MIGRATION_ERROR = "Migration Failed.";
    private string _message = msg;
    public new string Message { get => $"{MIGRATION_ERROR} {_message}";}

    private string DebuggerDisplay => ToString();
}