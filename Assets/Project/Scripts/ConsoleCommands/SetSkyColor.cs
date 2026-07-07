using UnityEngine;

public class SetSkyColor : IConsoleCommand
{
    public string Name => "SetSkyColor";

    public string[] Aliases => new string[] { "SetSkyboxColor" };

    public string Description => "Sets environment background color.";

    public string Syntax => "setskycolor <value>";

    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.Log($"syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError($"Missing argument: <value>");
            return;
        }

        bool argumentValid;

        LightningManager.Instance.SkyboxColors.TryGetValue(args[0], out string value);
        argumentValid = ColorUtility.TryParseHtmlString(value, out Color color);

        if (!argumentValid)
        {
            argumentValid = ColorUtility.TryParseHtmlString(args[0], out color);
            if (!argumentValid)
            {
                ConsoleManager.Instance.LogError($"Invalid argument <value>. Expected: clear_sky | horizon_sun | sunset_glow | sunset_dusk | overcast | toxic_neon | cyber_night | nebulla_pink | alien_dawn | #RRGGBB | #RRGGBBAA");
                return;
            }
        }

        LightningManager.Instance.SetEnvironmentBackgroundColor(color);
    }
}