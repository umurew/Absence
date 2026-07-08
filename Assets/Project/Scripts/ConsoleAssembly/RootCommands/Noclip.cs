using UnityEngine;

public class Noclip : IRootCommand
{
    public string Name => "Noclip";
    public string[] Aliases => new string[] { };
    public string Description => "Toggles clipping through.";
    public string Syntax => "noclip";
    public void Execute(string[] args)
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError($"Could not find any GameObject with tag: \"Player\"");
            ConsoleBridge.LogError($"Encountered unexpected state while executing command.");
            return;
        }

        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.ToggleNoclip();
    }
}