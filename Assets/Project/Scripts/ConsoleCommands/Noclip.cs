using UnityEngine;

public class Noclip : IConsoleCommand
{
    public string Name => "Noclip";

    public string[] Aliases => new string[] { };

    public string Description => "Toggles noclip";

    public string Syntax => "noclip";

    public void Execute(string[] args)
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        playerMovement.ToggleNoclip();
    }
}