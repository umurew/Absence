using UnityEngine;

public class SetSpeed : IConsoleCommand
{
    public string Name => "SetSpeed";
    public string[] Aliases => new string[] { };
    public string Description => "Sets player speed";
    public string Syntax => "setspeed <value>";
    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.SimulateCommand("Help", new string[] { "SetSpeed" });
            return;
        }

        bool argumentValid = int.TryParse(args[0], out int speed);
        if (!argumentValid)
        {
            ConsoleManager.Instance.SimulateCommand("Help", new string[] { "SetSpeed" });
            ConsoleManager.Instance.LogError($"Invalid value: \"{args[0]}\"");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        playerMovement.moveSpeed = speed;
    }
}