using System;

public enum ArgumentType
{
    String,
    Int,
    Float,
    Boolean,
    Vector3
}

public struct ArgumentDescriptor
{
    public string Name { get; }
    public bool IsOptional { get; }
    public ArgumentType Type { get; }
    public object[] AvailableOptions { get; }
    public ArgumentDescriptor(string name, ArgumentType type, object[] availableOptions = null, bool isOptional = false)
    {
        Name = name;
        Type = type;
        AvailableOptions = availableOptions;
        IsOptional = isOptional;
    }
    public readonly string DisplayString()
    {
        string openSymbol = IsOptional ? "[" : "<";
        string closeSymbol = IsOptional ? "]" : ">";

        return $"{openSymbol}{Name}:{Type}{closeSymbol}";
    }
}