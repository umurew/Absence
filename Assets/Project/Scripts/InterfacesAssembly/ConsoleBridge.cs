using System;

public static class ConsoleBridge
{
    public static event Action<string, bool> OnLog;
    public static event Action<string, bool> OnLogError;

    public static void Log(string message, bool printTimestamp = false) => OnLog?.Invoke(message, printTimestamp);
    public static void LogError(string message, bool printTimestamp = false) => OnLogError?.Invoke(message, printTimestamp);
}