using System.Collections.Generic;

public interface IConsoleService
{
    void Log(string text);
    void LogError(string text);
    void LogMissingArgumentError(string commandSyntax, string argumentName, string availableOptions);
    void LogInvalidArgumentError(string commandSyntax, string argumentName, string input, string availableOptions);
    void RegisterCommand(ICommand command);
    public void Clear();
    public bool TryGetCommand(string commandName, out ICommand command);
    public IEnumerable<ICommand> GetAllCommands();
}