using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Noclip : IRootCommand
{
    public string Name => "Noclip";
    public string[] Aliases => new string[] { };
    public string Description => "Toggles clipping through.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => Array.Empty<ArgumentDescriptor>();

    public void Execute(string[] args)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError($"Could not find any GameObject with tag: \"Player\"");
            ConsoleManager.Instance.LogError($"Encountered unexpected error while executing command.");
            return;
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ToggleNoclip();
    }

    public List<string> GetSuggestions(string[] args) => null;
}