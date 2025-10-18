
using System.Collections.Generic;

[DynamicClass(EDynamicSerial.LevelCommand)]
public class LevelCommand
{
    private List<LevelCommand> subCmds = new();
    /// <summary>
    /// 运行时生成的LevelCommand
    /// </summary>
    /// <param name="cmd"></param>
    public void AddSubCmd(LevelCommand cmd)
    {
        subCmds.Add(cmd);

    }
    /// <summary>
    /// 取出所有运行时生成的LevelCommand
    /// </summary>
    public List<LevelCommand> PickSub()
    {
        List<LevelCommand> res = new(subCmds);
        subCmds.Clear();
        return res;
    }
}