using System;
using System.Linq;

public class ClearCommand : IRootCommand
{
    public string Name => "Clear";
    public string[] Aliases => new string[] { };
    public string Description => "Clear the console.";
    public string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(argument => argument.GetDisplayString()))}";
    public ArgumentDescription[] Arguments => Array.Empty<ArgumentDescription>();

    private IConsoleService consoleService;

    public void Initialize(IConsoleService consoleService)
    {
        this.consoleService = consoleService;
    }

    public void Execute(string[] args) => consoleService.Clear();
}