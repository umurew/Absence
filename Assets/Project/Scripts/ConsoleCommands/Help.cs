public class Help : IConsoleCommand
{
    public string Name => "Help";
    public string[] Aliases => new string[] { "h", "commands" };
    public string Description => "Lists all available commands.";
    public string Syntax => "help | help <command name>";
    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            var commands = ConsoleManager.Instance.GetAllCommands();
            string output = string.Empty;

            foreach (var command in commands)
            {
                string aliasText = string.Empty;
                if (command.Aliases != null && command.Aliases.Length > 0)
                    aliasText = $"({string.Join(", ", command.Aliases)})";

                output += $"<size=14><color={ConsoleManager.Instance.AccentColor}><b>{command.Name}</b> <i>{aliasText}</i></color></size>{ConsoleManager.Instance.NewLine}<color={ConsoleManager.Instance.PrimaryColor}><i>{command.Syntax}</i></color>{ConsoleManager.Instance.Break}<color={ConsoleManager.Instance.GrayedColor}>{command.Description}</color>{ConsoleManager.Instance.Break}";
            }

            ConsoleManager.Instance.Log(output);
            return;
        }

        string targetCommandName = args[0];
        if (ConsoleManager.Instance.TryGetCommand(targetCommandName.ToLower(), out IConsoleCommand targetCommand))
        {
            string aliasText = (targetCommand.Aliases != null && targetCommand.Aliases.Length > 0)
                ? $" ({string.Join(", ", targetCommand.Aliases)})"
                : string.Empty;

            string output = $"<size=14><color={ConsoleManager.Instance.AccentColor}><b>{targetCommand.Name}</b> <i>{aliasText}</i></color></size>{ConsoleManager.Instance.NewLine}<color={ConsoleManager.Instance.PrimaryColor}><i>{targetCommand.Syntax}</i></color>{ConsoleManager.Instance.Break}<color={ConsoleManager.Instance.GrayedColor}>{targetCommand.Description}</color>";
            ConsoleManager.Instance.Log(output);
        }
        else
        {
            ConsoleManager.Instance.LogError($"Unknown command: \"{targetCommandName}\". Type \"help\" to see a full list of commands.");
        }
    }
}