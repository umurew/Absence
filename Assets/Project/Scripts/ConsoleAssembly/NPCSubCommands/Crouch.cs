using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crouch : ICommand
{
    public string Name => "Crouch";
    public string[] Aliases => new string[] { };
    public string Description => "Set if NPC is crouching.";
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

    public List<string> GetSuggestions(string[] args) => null;
}