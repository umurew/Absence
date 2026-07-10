public class Set : CommandWithSubCommands, IRootCommand
{
    public override string Name => "Set";
    public override string[] Aliases => new string[] { };
    public override string Description => "Sets various game parameters.";

    public Set()
    {
        RegisterSubCommand(new Speed());
        RegisterSubCommand(new SkyColor());
        RegisterSubCommand(new NPC());
    }
}