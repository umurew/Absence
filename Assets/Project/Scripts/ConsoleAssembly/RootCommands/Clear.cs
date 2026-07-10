using System;
using System.Collections.Generic;
using System.Linq;

public class Clear : IRootCommand
{
    public string Name => "Clear";
    public string[] Aliases => new string[] { };
    public string Description => "Clears the console.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(a => a.DisplayString()))}";
    public ArgumentDescriptor[] Arguments => Array.Empty<ArgumentDescriptor>();

    public void Execute(string[] args) => ConsoleManager.Instance.ClearConsole();

    public List<string> GetSuggestions(string[] args) => null;
}