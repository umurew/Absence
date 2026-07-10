public class NPC : CommandWithSubCommands
{
    public override string Name => "NPC";
    public override string[] Aliases => new string[] {  };
    public override string Description => "Sets properties of NPC in order to debug.";

    public NPC()
    {
        RegisterSubCommand(new Crouch());
        RegisterSubCommand(new Sprint());
        RegisterSubCommand(new Move());
        RegisterSubCommand(new Jump());
    }
}