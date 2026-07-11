using System;
using System.Linq;

public interface ICommand
{
    string Name { get; }
    string[] Aliases { get; }
    string Description { get; }
    string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(argument => argument.GetDisplayString()))}";
    ArgumentDescription[] Arguments => Array.Empty<ArgumentDescription>();
    void Initialize(IConsoleService consoleService);
    void Execute(string[] args);
}

public interface IRootCommand : ICommand { }