using log4net;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crouch : ICommand
{
    public string Name => "Crouch";
    public string[] Aliases => new string[] { };
    public string Description => "Set if NPCs are crouching.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => new ArgumentDescriptor[]
    {
        new("value", ArgumentType.Boolean)
    };

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            ConsoleManager.Instance.LogMissingArgument(Syntax, Arguments[0].DisplayString(), Arguments[0].Type.ToString());
            return;
        }

        string input = args[0];

        if (!Parser.TryParseBoolean(input, out bool value))
        {
            ConsoleManager.Instance.LogInvalidArgument(Syntax, Arguments[0].DisplayString(), input, Arguments[0].Type.ToString());
            return;
        }

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        if (npcs.Length == 0)
        {
            ConsoleManager.Instance.Log("Could not find any GameObject with tag: \"NPC\"");
            return;
        }

        foreach (GameObject npc in npcs)
        {
            if (!npc.TryGetComponent<NPCMovement>(out NPCMovement npcMovement))
                return;

            npcMovement.blackboard.SetCrouching(value);
        }
    }

    public List<string> GetSuggestions(string[] args) => null;
}