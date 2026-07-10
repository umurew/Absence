using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jump : ICommand
{
    public string Name => "Jump";
    public string[] Aliases => new string[] {  };
    public string Description => "Set if NPC jump FUCK YOU.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => Array.Empty<ArgumentDescriptor>();

    public void Execute(string[] args)
    {
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

            npcMovement.blackboard.RequestJump();
        }
    }

    public List<string> GetSuggestions(string[] args)
    {
        return null;
    }
}