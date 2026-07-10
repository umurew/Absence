using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Help : IRootCommand
{
    public string Name => "Help";
    public string[] Aliases => new string[] { };
    public string Description => "Lists all available commands or provides detailed help for a specific command.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => new ArgumentDescriptor[]
    {
        new("command", ArgumentType.String, ConsoleManager.Instance.GetAllCommands().Select(command => command.Name).ToArray(), true)
    };

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            var commands = ConsoleManager.Instance.GetAllCommands();
            StringBuilder output = new();

            foreach (var command in commands)
                AppendCommandDetails(output, command, 0);

            ConsoleManager.Instance.Log(output.ToString());
            return;
        }

        string targetCommandName = args[0];
        if (ConsoleManager.Instance.TryGetCommand(targetCommandName.ToLower(), out ICommand targetCommand))
        {
            StringBuilder output = new();

            AppendCommandDetails(output, targetCommand, 0);
            ConsoleManager.Instance.Log(output.ToString());
        }
        else
            ConsoleManager.Instance.LogError($"Unknown command: \"{targetCommandName}\". Type \"help\" to see a full list of commands.");
    }

    private void AppendCommandDetails(StringBuilder stringBuilder, ICommand command, int indentLevel)
    {
        var uiColors = ColorProvider.UIColors;
        var inputManager = InputManager.Instance;

        string tabs = string.Concat(Enumerable.Repeat(inputManager.Tab, indentLevel));
        string aliasText = (command.Aliases != null && command.Aliases.Length > 0)
            ? $" ({string.Join(", ", command.Aliases)})"
            : string.Empty;

        int fontSize = indentLevel == 0 ? 14 : 13;

        stringBuilder.Append(tabs).Append($"<size={fontSize}><color={uiColors.AccentColor}><b>{command.Name}</b><i>{aliasText}</i></color></size>").Append(inputManager.NewLine);
        stringBuilder.Append(tabs).Append($"<color={uiColors.PrimaryColor}><i>{command.Syntax}</i></color>").Append(inputManager.NewLine);
        stringBuilder.Append(tabs).Append($"<color={uiColors.GrayedColor}>{command.Description}</color>").Append(inputManager.Break);

        if (command is ICommandWithSubCommands compositeCommand)
        {
            var properties = compositeCommand.GetUniqueSubCommands();
            if (properties != null)
            {
                foreach (var prop in properties)
                    AppendCommandDetails(stringBuilder, prop, indentLevel + 1);
            }
        }
    }

    public List<string> GetSuggestions(string[] args)
    {
        if (args.Length > 1)
            return null;

        string input = args.Length == 1 ? args[0].ToLower() : string.Empty;

        List<string> suggestions = new();
        IEnumerable<ICommand> commands = ConsoleManager.Instance.GetAllCommands();

        foreach (ICommand command in commands)
        {
            string loweredName = command.Name.ToLower();
            if (loweredName.StartsWith(input))
                suggestions.Add(loweredName);

            if (command.Aliases != null && command.Aliases.Length > 0)
            {
                foreach (string alias in command.Aliases)
                {
                    string loweredAlias = alias.ToLower();
                    if (loweredAlias.StartsWith(input))
                        suggestions.Add(loweredAlias);
                }
            }
        }

        return suggestions.Count > 0 ? suggestions : null;
    }
}