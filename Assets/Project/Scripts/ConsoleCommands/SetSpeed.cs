using System.Collections.Generic;
using UnityEngine;

public class SetSpeed : IConsoleCommand
{
    public string Name => "SetSpeed";

    public string[] Aliases => new string[] { };

    public string Description => "Sets player speed";

    public string Syntax => "setspeed <type> <value>";

    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.Log($"syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError($"Missing argument: <type>");
            return;
        }

        bool argumentValid;

        argumentValid = TryParse(args[0], out string type);
        if (!argumentValid)
        {
            ConsoleManager.Instance.LogError($"Invalid argument <type>. Expected: move | sprint | crouch | noclip");
            return;
        }

        if (args.Length < 2)
        {
            ConsoleManager.Instance.Log($"syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError($"Missing argument: <value>");
            return;
        }

        argumentValid = int.TryParse(args[1], out int speed);
        if (!argumentValid)
        {
            ConsoleManager.Instance.LogError($"Invalid argument <value>: Expected integer");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        switch (type)
        {
            case "move":
                playerMovement.moveSpeed = speed;
                break;
            case "sprint":
                playerMovement.sprintSpeed = speed;
                break;
            case "crouch":
                playerMovement.crouchSpeed = speed;
                break;
            case "noclip":
                playerMovement.noclipSpeed = speed;
                break;

        }
    }

    private bool TryParse(string s, out string result)
    {
        HashSet<string> validArguments = new() { "move", "sprint", "crouch", "noclip" };
        string processedString = s?.Trim().ToLower() ?? string.Empty;

        if (validArguments.Contains(processedString))
        {
            result = processedString;
            return true;
        }

        result = string.Empty;
        return false;
    }
}