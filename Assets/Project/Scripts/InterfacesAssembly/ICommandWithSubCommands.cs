using System.Collections.Generic;
using System.Linq;

public interface ICommandWithSubCommands : ICommand
{
    IEnumerable<ICommand> GetUniqueSubCommands();
}

public abstract class CommandWithSubCommands : ICommandWithSubCommands
{
    public abstract string Name { get; }
    public abstract string[] Aliases { get; }
    public abstract string Description { get; }
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public virtual ArgumentDescriptor[] Arguments => new ArgumentDescriptor[]
    {
        new("subcommand", ArgumentType.String)
    };

    private readonly Dictionary<string, ICommand> _subCommands = new();

    protected string SubCommandsDisplayText => string.Join(" | ", GetUniqueSubCommands().Select(subCommand => subCommand.Name.ToLower()));

    public IEnumerable<ICommand> GetUniqueSubCommands() => _subCommands.Values.Distinct();

    protected void RegisterSubCommand(ICommand subCommand)
    {
        // Handle names
        if (!_subCommands.ContainsKey(subCommand.Name.ToLower()))
            _subCommands.Add(subCommand.Name.ToLower(), subCommand);

        // Handle aliases
        if (subCommand.Aliases != null)
        {
            foreach (string alias in subCommand.Aliases)
            {
                string cleanAlias = alias.Trim().ToLower();
                if (!string.IsNullOrEmpty(cleanAlias) && !_subCommands.ContainsKey(cleanAlias))
                    _subCommands.Add(cleanAlias, subCommand);
            }
        }
    }

    public virtual void Execute(string[] args)
    {
        // Handle null <subcommand> argument
        if (args.Length == 0)
        {
            ConsoleBridge.LogMissingArgument(Syntax, "subcommand", SubCommandsDisplayText);
            return;
        }

        // Handle executing sub-command
        string targetSubCommand = args[0];
        if (_subCommands.TryGetValue(targetSubCommand.ToLower(), out ICommand subCommand))
        {
            string[] subArgs = args.Skip(1).ToArray();
            subCommand.Execute(subArgs);
        }
        else
            ConsoleBridge.LogError($"Unknown sub-command: \"{targetSubCommand}\". Type \"help {Name.ToLower()}\" or enter \"{Name.ToLower()}\" for options.");
    }

    public virtual List<string> GetSuggestions(string[] args)
    {
        /// Return every sub-command's base name if nothing is written
        if (args == null || args.Length == 0)
            return GetUniqueSubCommands().Select(subCommand => subCommand.Name.ToLower()).ToList();

        string currentInput = args[0].ToLower();

        /// In case user has written a sub-command name or alias, for example: "set npc"
        if (_subCommands.TryGetValue(currentInput, out ICommand directMatchCommand))
        {
            string[] subArgs = args.Skip(1).ToArray();
            return directMatchCommand.GetSuggestions(subArgs);
        }

        /// In case user is writing a sub-command name or alias, for example: "set n"
        if (args.Length == 1)
        {
            List<string> suggestions = new();

            /// Since subCommand registry also adds aliases to _subCommands, we don't search for them
            foreach (var pair in _subCommands)
            {
                if (pair.Key.StartsWith(currentInput))
                    suggestions.Add(pair.Key);
            }

            return suggestions.Count > 0 ? suggestions : null;
        }

        /// In case user has written the sub-command name or alias correct and is now writing sub-commands' sub-commands, for example: "set npc wa"
        if (_subCommands.TryGetValue(currentInput, out ICommand nextCommand))
        {
            string[] subArgs = args.Skip(1).ToArray();
            return nextCommand.GetSuggestions(subArgs);
        }

        /// Return null in case none of above
        return null;
    }
}