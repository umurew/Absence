using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Move : ICommand
{
    public string Name => "Move";
    public string[] Aliases => new string[] {};
    public string Description => "Makes NPC move to the given point if available.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => new ArgumentDescriptor[]
    {
        new("value", ArgumentType.Vector3)
    };

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            ConsoleManager.Instance.LogMissingArgument(Syntax, Arguments[0].DisplayString(), Arguments[0].Type.ToString());
            return;
        }

        string destinationInput = args[0];

        if (!Parser.TryParseVector3(destinationInput, out Vector3 destination))
        {
            ConsoleManager.Instance.LogInvalidArgument(Syntax, Arguments[0].DisplayString(), destinationInput, Arguments[0].Type.ToString());
            return;
        }

        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        if (npcs.Length == 0)
        {
            ConsoleManager.Instance.Log("Could not find any GameObject with tag: \"NPC\"");
            return;
        }

        GameObject npc = npcs[0];

        if (!npc.TryGetComponent<NPCMovement>(out NPCMovement npcMovement))
            return;

        npcMovement.blackboard.SetTarget(destination);
        npcMovement.blackboard.OnTargetReached += TargetReached;
    }

    private void TargetReached(NPCBlackboard npcBlackboard, Vector3 destination)
    {
        npcBlackboard.OnTargetReached -= TargetReached;
        ConsoleManager.Instance.Log($"<color=#5ede4e><i>NPC reached {destination}</i></color>");
    }

    public List<string> GetSuggestions(string[] args) => null;
}