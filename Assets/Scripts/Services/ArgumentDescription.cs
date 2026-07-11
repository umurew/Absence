public enum ArgumentType
{
    String,
    Int,
    Float,
    Boolean,
    Vector3
}

public struct ArgumentDescription
{
    public string Name { get; }
    public bool IsOptional { get; }
    public ArgumentType Type { get; }
    public object[] AvailableOptions { get; }
    public ArgumentDescription(string name, ArgumentType type, object[] availableOptions = null, bool isOptional = false)
    {
        Name = name;
        Type = type;
        AvailableOptions = availableOptions;
        IsOptional = isOptional;
    }
    public readonly string GetDisplayString()
    {
        string openSymbol = IsOptional ? "[" : "<";
        string closeSymbol = IsOptional ? "]" : ">";

        return $"{openSymbol}{Name}:{Type}{closeSymbol}";
    }
}