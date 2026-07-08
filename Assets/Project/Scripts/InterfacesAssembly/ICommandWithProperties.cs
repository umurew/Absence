using System.Collections.Generic;
using System.Linq;

public interface ICommandWithProperties : ICommand
{
    IEnumerable<ICommand> GetUniqueProperties();
}

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
        // Handle names
        if (!_subProperties.ContainsKey(property.Name.ToLower()))
            _subProperties.Add(property.Name.ToLower(), property);

        // Handle aliases
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
        // Handle null <property> argument
        if (args == null || args.Length == 0)
        {
            ConsoleBridge.Log($"Syntax: <i>{Syntax}</i>");
            ConsoleBridge.LogError($"Missing argument: <property>. Expected: {PropertiesDisplayText}");
            return;
        }

        // Handle executing property
        string targetProperty = args[0].ToLower();
        if (_subProperties.TryGetValue(targetProperty, out ICommand property))
        {
            string[] subArgs = args.Skip(1).ToArray();
            property.Execute(subArgs);
        }
        else
            ConsoleBridge.LogError($"Unknown property: \"{args[0]}\". Type \"help {Name.ToLower()}\" or enter \"{Name.ToLower()}\" for options.");
    }

    public virtual List<string> GetSuggestions(string[] args)
    {
        /// Return every property's base name if nothing is written
        if (args == null || args.Length == 0)
            return GetUniqueProperties().Select(property => property.Name.ToLower()).ToList();

        string currentInput = args[0].ToLower();

        /// In case user has written a property name or alias, for example: "set npc"
        if (_subProperties.TryGetValue(currentInput, out ICommand directMatchCommand))
        {
            string[] subArgs = args.Skip(1).ToArray();
            return directMatchCommand.GetSuggestions(subArgs);
        }

        /// In case user is writing a property name or alias, for example: "set n"
        if (args.Length == 1)
        {
            List<string> suggestions = new();

            /// Since property registry also adds aliases to _subProperties, we don't search for them
            foreach (var pair in _subProperties)
            {
                if (pair.Key.StartsWith(currentInput))
                    suggestions.Add(pair.Key);
            }

            return suggestions.Count > 0 ? suggestions : null;
        }

        /// In case user has written the property name or alias correct and is now writing sub-properties, for example: "set npc wa"
        if (_subProperties.TryGetValue(currentInput, out ICommand nextCommand))
        {
            string[] subArgs = args.Skip(1).ToArray();
            return nextCommand.GetSuggestions(subArgs);
        }

        /// Return null in case none of above
        return null;
    }
}