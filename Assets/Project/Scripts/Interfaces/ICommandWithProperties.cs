using System.Collections.Generic;

public interface ICommandWithProperties : ICommand
{
    IEnumerable<ICommand> GetUniqueProperties();
}