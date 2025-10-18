using System.Collections.Generic;
using System.Linq;

public class CommandGroup : LevelCommand
{
    public List<LevelCommand> Cmds { get; private set; } = new();
    public CommandGroup(params LevelCommand[] cmds)
    {
        Cmds = cmds.ToList();
    }
}