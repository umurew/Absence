using System;

public static class ConsoleBridge
{
    public static event Action<string, bool> OnLog;
    public static event Action<string, bool> OnLogError;
    public static event Action<string, string, string, bool> OnLogMissingArgument;
    public static event Action<string, string, string, string, bool> OnLogInvalidArgument;

    public static void Log(string message, bool printTimestamp = false) => OnLog?.Invoke(message, printTimestamp);
    public static void LogError(string message, bool printTimestamp = false) => OnLogError?.Invoke(message, printTimestamp);
    public static void LogMissingArgument(string commandSyntax, string argumentName, string availableOptions, bool printTimestamp = false) => OnLogMissingArgument?.Invoke(commandSyntax, argumentName, availableOptions, printTimestamp);
    public static void LogInvalidArgument(string commandSyntax, string argumentName, string invalidInput, string availableOptions, bool printTimestamp = false) => OnLogInvalidArgument?.Invoke(commandSyntax, argumentName, invalidInput, availableOptions, printTimestamp);
}