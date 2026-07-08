using System.Collections.Generic;
using System.Linq;

public class Spawn : IRootCommand
{
    public string Name => "Spawn";
    public string[] Aliases => new string[] { "Create" };
    public string Description => "Instansiates requested object if available.";
    public string Syntax => "spawn <value>";

    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.Log($"Syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError("Missing argument: <value>. Use TAB completion to see available values.");
            return;
        }

        if (!Spawner.Instance.TrySpawnObject(args[0]))
            ConsoleManager.Instance.LogError($"Unknown object could not spawn: {args[0]}");
    }

    public List<string> GetSuggestions(string[] args)
    {
        /// In case user did not write <value> or has written <value>
        if (args == null || args.Length > 1)
            return null;

        /// In case user is writing <value>
        string input = args.Length == 1 ? args[0].ToLower() : string.Empty;
        var objectKeys = Spawner.Instance.GetAvailableObjects();

        var matches = objectKeys.Where(key => key.ToLower().StartsWith(input)).ToList();
        return matches.Count > 0 ? matches : null;
    }
}