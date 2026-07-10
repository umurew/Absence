using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawn : IRootCommand
{
    public string Name => "Spawn";
    public string[] Aliases => new string[] { "Create" };
    public string Description => "Instansiates requested object if available.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => new ArgumentDescriptor[]
    {
        new("value", ArgumentType.String, null),
        new("position", ArgumentType.Vector3, null, true)
    };

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            ConsoleManager.Instance.LogMissingArgument(Syntax, Arguments[0].DisplayString(), null);
            return;
        }

        string targetObject = args[0];
        string positionInput = args.Length >= 2 ? args[1] : "player";

        if (!Parser.TryParseVector3(positionInput, out Vector3 spawnPosition))
        {
            ConsoleManager.Instance.LogInvalidArgument(Syntax, Arguments[1].DisplayString(), positionInput, Arguments[1].Type.ToString());
            return;
        }

        if (!Spawner.Instance.TrySpawnObject(targetObject, spawnPosition))
            ConsoleManager.Instance.LogError($"Prefab '{targetObject}' not found in registry or instantiation failed.");
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