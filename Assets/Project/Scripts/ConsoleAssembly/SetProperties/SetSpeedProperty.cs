using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetSpeedProperty : ICommand
{
    public string Name => "Speed";
    public string[] Aliases => new string[] { };
    public string Description => "Sets speed properties of Player.";
    public string Syntax => "set speed <type> <value>";
    
    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.Log($"Syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError("Missing argument: <type>. Expected: move | sprint | crouch | noclip");
            return;
        }

        string type = args[0].ToLower();
        HashSet<string> validTypes = new() { "move", "sprint", "crouch", "noclip" };

        if (!validTypes.Contains(type))
        {
            ConsoleManager.Instance.LogError("Invalid argument <type>. Expected: move | sprint | crouch | noclip");
            return;
        }

        if (args.Length < 2)
        {
            ConsoleManager.Instance.Log($"Syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError("Missing argument: <value>");
            return;
        }

        if (!int.TryParse(args[1], out int value))
        {
            ConsoleManager.Instance.LogError("Invalid argument <value>: Expected integer");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError($"Could not find any GameObject with tag: \"Player\"");
            ConsoleManager.Instance.LogError($"Encountered unexpected state while executing command.");
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        switch (type)
        {
            case "move":
                playerMovement.moveSpeed = value;
                break;
            case "sprint":
                playerMovement.sprintSpeed = value;
                break;
            case "crouch":
                playerMovement.crouchSpeed = value;
                break;
            case "noclip":
                playerMovement.noclipSpeed = value;
                break;
        }
    }

    public List<string> GetSuggestions(string[] args)
    {
        /// In case user did not write <type> or is writing <value>
        if (args == null || args.Length > 1)
            return null;

        /// In case user is writing <type>
        string input = args.Length == 1 ? args[0].ToLower() : string.Empty;
        List<string> types = new() { "move", "sprint", "crouch", "noclip" };

        var matches = types.Where(type => type.StartsWith(input)).ToList();
        return matches.Count > 0 ? matches : null;
    }
}