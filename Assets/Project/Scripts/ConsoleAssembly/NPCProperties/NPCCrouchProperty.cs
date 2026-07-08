using System.Collections.Generic;
using UnityEngine;

public class NPCCrouchProperty : ICommand
{
    public string Name => "Crouch";
    public string[] Aliases => new string[] { };
    public string Description => "Set if NPC is crouching.";
    public string Syntax => "set npc crouch <value>";

    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.Log($"Syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError("Missing argument: <value>. Expected: true | false | 0 | 1");
            return;
        }

        string input = args[0];

        if (!bool.TryParse(input, out bool value))
        {
            if (int.TryParse(input, out int intValue))
            {
                if (intValue < 0 || intValue > 1)
                {
                    ConsoleManager.Instance.LogError("Invalid argument <value>: Expected: 0 | 1");
                    return;
                }

                value = (intValue == 1);
            }
            else
            {
                ConsoleManager.Instance.Log($"Syntax: <i>{Syntax}</i>");
                ConsoleManager.Instance.LogError("Invalid argument <value>. Expected: true | false | 0 | 1");
                return;
            }
        }

        GameObject npc = GameObject.FindGameObjectWithTag("NPC");

        if (npc == null)
        {
            Debug.LogError("Could not find any GameObject with tag: \"NPC\"");
            return;
        }

        if (!npc.TryGetComponent<NPCMovement>(out NPCMovement npcMovement))
            return;

        npcMovement.isCrouching = value;
    }

    public List<string> GetSuggestions(string[] args)
    {
        return null;
    }
}