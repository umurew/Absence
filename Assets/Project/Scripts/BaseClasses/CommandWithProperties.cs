using System.Collections.Generic;
using System.Linq;

public abstract class CommandWithProperties : ICommandWithProperties
{
    public abstract string Name { get; }
    public abstract string[] Aliases { get; }
    public abstract string Description { get; }
    public abstract string Syntax { get; }

    private readonly Dictionary<string, ICommand> _subProperties = new();

    protected string PropertiesDisplayText => string.Join(" | ", GetUniqueProperties().Select(property => property.Name.ToLower()));

    public IEnumerable<ICommand> GetUniqueProperties() => _subProperties.Values.Distinct();

    protected void RegisterProperty(ICommand property)
    {
        if (!_subProperties.ContainsKey(property.Name.ToLower()))
            _subProperties.Add(property.Name.ToLower(), property);

        if (property.Aliases != null)
        {
            foreach (string alias in property.Aliases)
            {
                string cleanAlias = alias.Trim().ToLower();
                if (!string.IsNullOrEmpty(cleanAlias) && !_subProperties.ContainsKey(cleanAlias))
                    _subProperties.Add(cleanAlias, property);
            }
        }
    }

    public virtual void Execute(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            ConsoleManager.Instance.Log($"Syntax: <i>{Syntax}</i>");
            ConsoleManager.Instance.LogError($"Missing argument: <property>. Expected: {PropertiesDisplayText}");
            return;
        }

        string targetProperty = args[0].ToLower();

        if (_subProperties.TryGetValue(targetProperty, out ICommand property))
        {
            string[] subArgs = args.Skip(1).ToArray();
            property.Execute(subArgs);
        }
        else
            ConsoleManager.Instance.LogError($"Unknown property: \"{args[0]}\". Type \"help {Name.ToLower()}\" or enter \"{Name.ToLower()}\" for options.");
    }
}