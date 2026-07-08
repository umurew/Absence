public class Set : CommandWithProperties, IRootCommand
{
    public override string Name => "Set";
    public override string[] Aliases => new string[] { };
    public override string Description => "Sets various game parameters.";
    public override string Syntax => "set <property> <...args>";

    public Set()
    {
        RegisterProperty(new SetSpeedProperty());
        RegisterProperty(new SetSkyColorProperty());
        RegisterProperty(new SetNPCProperty());
    }
}