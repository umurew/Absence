using System.Linq;
using System.Text;

public class Help : IRootCommand
{
    public string Name => "Help";
    public string[] Aliases => new string[] { };
    public string Description => "Lists all available commands or provides detailed help for a specific command.";
    public string Syntax => "help | help <command>";

    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            var commands = ConsoleManager.Instance.GetAllCommands();
            StringBuilder output = new();

            foreach (var command in commands)
                AppendCommandDetails(output, command, 0);

            ConsoleManager.Instance.Log(output.ToString());
            return;
        }

        string targetCommandName = args[0].ToLower();
        if (ConsoleManager.Instance.TryGetCommand(targetCommandName, out ICommand targetCommand))
        {
            StringBuilder output = new();

            AppendCommandDetails(output, targetCommand, 0);
            ConsoleManager.Instance.Log(output.ToString());
        }
        else
            ConsoleManager.Instance.LogError($"Unknown command: \"{args[0]}\". Type \"help\" to see a full list of commands.");
    }

    private void AppendCommandDetails(StringBuilder stringBuilder, ICommand command, int indentLevel)
    {
        var uiColors = ColorProvider.Instance.UIColors;
        var inputManager = InputManager.Instance;

        string tabs = string.Concat(Enumerable.Repeat(inputManager.Tab, indentLevel));
        string aliasText = (command.Aliases != null && command.Aliases.Length > 0)
            ? $" ({string.Join(", ", command.Aliases)})"
            : string.Empty;

        int fontSize = indentLevel == 0 ? 14 : 13;

        stringBuilder.Append(tabs).Append($"<size={fontSize}><color={uiColors.AccentColor}><b>{command.Name}</b><i>{aliasText}</i></color></size>").Append(inputManager.NewLine);
        stringBuilder.Append(tabs).Append($"<color={uiColors.PrimaryColor}><i>{command.Syntax}</i></color>").Append(inputManager.Break);
        stringBuilder.Append(tabs).Append($"<color={uiColors.GrayedColor}>{command.Description}</color>").Append(inputManager.Break);

        if (command is ICommandWithProperties compositeCommand)
        {
            var properties = compositeCommand.GetUniqueProperties();
            if (properties != null)
            {
                foreach (var prop in properties)
                    AppendCommandDetails(stringBuilder, prop, indentLevel + 1);
            }
        }
    }
}