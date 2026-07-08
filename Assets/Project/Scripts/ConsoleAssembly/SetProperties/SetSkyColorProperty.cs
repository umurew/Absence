using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetSkyColorProperty : ICommand
{
    public string Name => "SkyColor";
    public string[] Aliases => new string[] { "Sky", "SkyboxColor", "EnvironmentBackgroundColor" };
    public string Description => "Sets the background color of the environment.";
    public string Syntax => "set skycolor <value>";

    private string ColorsDisplayText => string.Join(" | ", LightningManager.Instance.GetAvailableColorKeys());

    public void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.Log($"Syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError($"Missing argument: <type>. Expected: {ColorsDisplayText}");
            return;
        }

        string colorInput = args[0];
        bool argumentValid;

        LightningManager.Instance.SkyboxColors.TryGetValue(colorInput, out string hexValue);
        argumentValid = ColorUtility.TryParseHtmlString(hexValue, out Color color);

        if (!argumentValid)
        {
            argumentValid = ColorUtility.TryParseHtmlString(colorInput, out color);
            if (!argumentValid)
            {
                ConsoleManager.Instance.LogError($"Invalid argument <value>. Expected: {ColorsDisplayText} | #RRGGBB | #RRGGBBAA");
                return;
            }
        }

        LightningManager.Instance.SetEnvironmentBackgroundColor(color);
    }

    public List<string> GetSuggestions(string[] args)
    {
        /// In case user did not write <value> or has written <value>
        if (args == null || args.Length > 1)
            return null;

        /// In case user is writing <value>
        string input = args.Length == 1 ? args[0].ToLower() : string.Empty;
        var colorKeys = LightningManager.Instance.GetAvailableColorKeys();

        var matches = colorKeys.Where(color => color.ToLower().StartsWith(input)).ToList();
        return matches.Count > 0 ? matches : null;
    }
}