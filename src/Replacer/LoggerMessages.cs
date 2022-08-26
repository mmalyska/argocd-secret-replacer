namespace Replacer;

using Microsoft.Extensions.Logging;

public static partial class LoggerMessages
{
    [LoggerMessage(1, LogLevel.Error, "{errors}")]
    public static partial void LogErrorWrongParams(ILogger logger, string errors);
}
