using System.Collections.Generic;
using System.Linq;

public interface ICommandParent : ICommand
{
    IEnumerable<ICommand> GetUniqueCommands();
}

public abstract class CommandParent : ICommandParent
{
    public abstract string Name { get; }
    public abstract string[] Aliases { get; }
    public abstract string Description { get; }
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(argument => argument.GetDisplayString()))}";
    public virtual ArgumentDescription[] Arguments => new ArgumentDescription[]
    {
        new("command", ArgumentType.String)
    };

    private string CommandsDisplayString => string.Join(" | ", GetUniqueCommands().Select(CommandParent => CommandParent.Name.ToLower()));
    private readonly Dictionary<string, ICommand> _commands = new();
    private IConsoleService consoleService;

    public IEnumerable<ICommand> GetUniqueCommands() => _commands.Values.Distinct();

    public void Initialize(IConsoleService consoleService)
    {
        this.consoleService = consoleService;
    }

    public virtual void Execute(string[] args)
    {
        // Handle null <command> argument
        if (args.Length == 0)
        {
            consoleService.LogMissingArgumentError(Syntax, Arguments[0].Name, CommandsDisplayString);
            return;
        }

        // Handle executing command
        string targetCommand = args[0];
        if (_commands.TryGetValue(targetCommand.ToLower(), out ICommand command))
        {
            string[] subArgs = args.Skip(1).ToArray();
            command.Execute(subArgs);
        }
        else
            consoleService.LogError($"Unknown command: \"{targetCommand}\". Type \"help {Name.ToLower()}\" for options.");
    }

    protected void RegisterCommand(ICommand command)
    {
        if (!_commands.ContainsKey(command.Name.ToLower()))
            _commands.Add(command.Name.ToLower(), command);

        if (command.Aliases != null)
        {
            foreach (string alias in command.Aliases)
            {
                string cleanAlias = alias.Trim().ToLower();

                if (!string.IsNullOrEmpty(cleanAlias) && !_commands.ContainsKey(cleanAlias))
                    _commands.Add(cleanAlias, command);
            }
        }
    }
}