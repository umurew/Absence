using System;
using System.Collections.Generic;
using System.Linq;

public interface ICommand
{
    string Name { get; }
    string[] Aliases { get; }
    string Description { get; }
    string Syntax => $"{Name.ToLower()} {string.Join(' ', Arguments.Select(argument => argument.DisplayString()))}";
    ArgumentDescriptor[] Arguments => Array.Empty<ArgumentDescriptor>();
    void Execute(string[] args);
    List<string> GetSuggestions(string[] args);
}