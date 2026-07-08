public class SetNPCProperty : CommandWithProperties
{
    public override string Name => "NPC";
    public override string[] Aliases => new string[] {  };
    public override string Description => "Sets properties of NPC in order to debug.";
    public override string Syntax => "set npc <property> <value>";

    public SetNPCProperty()
    {
        RegisterProperty(new NPCWanderProperty());
        RegisterProperty(new NPCJumpProperty());
    }
}