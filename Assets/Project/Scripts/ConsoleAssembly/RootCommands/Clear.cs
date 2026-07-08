using System.Collections.Generic;

public class Clear : IRootCommand
{
    public string Name => "Clear";
    public string[] Aliases => new string[] { };
    public string Description => "Clears the console.";
    public string Syntax => "clear";

    public void Execute(string[] args)
    {
        ConsoleManager.Instance.ClearConsole();
    }

    public List<string> GetSuggestions(string[] args)
    {
        return null;
    }
}