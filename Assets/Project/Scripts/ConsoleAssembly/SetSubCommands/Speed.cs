using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Speed : ICommand
{
    public string Name => "Speed";
    public string[] Aliases => new string[] { };
    public string Description => "Sets speed properties of Player.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => new ArgumentDescriptor[]
    {
        new("type", ArgumentType.String, new string[] { "move", "sprint", "crouch", "noclip" }),
        new("value", ArgumentType.Float)
    };

    public void Execute(string[] args)
    {
        if ( args.Length == 0)
        {
            ConsoleManager.Instance.LogMissingArgument(Syntax, Arguments[0].DisplayString(), string.Join(" | ", Arguments[0].AvailableOptions));
            return;
        }

        string type = args[0];

        if (!Arguments[0].AvailableOptions.Contains(type.ToLower()))
        {
            ConsoleManager.Instance.LogInvalidArgument(Syntax, Arguments[0].DisplayString(), type, string.Join(" | ", Arguments[0].AvailableOptions));
            return;
        }

        if (args.Length < 2)
        {
            ConsoleManager.Instance.LogMissingArgument(Syntax, Arguments[1].DisplayString(), Arguments[1].Type.ToString());
            return;
        }

        if (!int.TryParse(args[1], out int value))
        {
            ConsoleManager.Instance.LogInvalidArgument(Syntax, Arguments[1].DisplayString(), args[1], Arguments[1].Type.ToString());
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

        var matches = ((string[])Arguments[0].AvailableOptions).Where(type => type.StartsWith(input)).ToList();
        return matches.Count > 0 ? matches : null;
    }
}