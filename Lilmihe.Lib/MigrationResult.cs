using System;
using System.Collections.Generic;

namespace Lilmihe
{
    public class MigrationResult
    {
        public bool Success { get; set; } = false;

        public string? FailedCommand { get; set; }

        public string? FailedFile { get; set; }

        public List<string> SuccessFiles { get; set; } = new List<string>();

        public string Message { get; set; } = string.Empty;

        public Exception? Error { get; set; }
    }
}
