public interface ICommand
{
    string Name { get; }
    string[] Aliases { get; }
    string Description { get; }
    string Syntax { get; }
    void Execute(string[] args);
}