using System.Collections.Generic;
using UnityEngine;

public class NPCJumpProperty : ICommand
{
    public string Name => "Jump";
    public string[] Aliases => new string[] { };
    public string Description => "Trigger NPC to jump if available.";
    public string Syntax => "set npc jump";

    public void Execute(string[] args)
    {
        GameObject npc = GameObject.FindGameObjectWithTag("NPC");

        if (npc == null)
        {
            Debug.LogError("Could not find any GameObject with tag: \"NPC\"");
            return;
        }

        if (!npc.TryGetComponent<NPCMovement>(out NPCMovement npcMovement))
            return;

        npcMovement.Jump();
    }

    public List<string> GetSuggestions(string[] args)
    {
        return null;
    }
}