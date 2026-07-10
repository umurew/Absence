using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkyColor : ICommand
{
    public string Name => "SkyColor";
    public string[] Aliases => new string[] { "Sky", "SkyboxColor", "EnvironmentBackgroundColor" };
    public string Description => "Sets the background color of the environment.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments = new ArgumentDescriptor[]
    {
        new("value", ArgumentType.String, LightningManager.Instance.GetAvailableColorKeys().Append("#RRGGBB").Append("#RRGGBBAA").ToArray())
    };

    private string ColorsDisplayText => string.Join(" | ", LightningManager.Instance.GetAvailableColorKeys());

    public void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            ConsoleManager.Instance.LogMissingArgument(Syntax, Arguments[0].DisplayString(), string.Join(" | ", Arguments[0].AvailableOptions));
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
                ConsoleManager.Instance.LogInvalidArgument(Syntax, Arguments[0].DisplayString(), colorInput, string.Join(" | ", Arguments[0].AvailableOptions));
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