public class AddLeverageBuffCMD : LevelCommand
{
    public int StackCount { get; set; }
    public float Multiplier { get; set; }
    public int Duration { get; set; }
    public bool RefreshDuration { get; set; }

    public AddLeverageBuffCMD(
        int stackCount = 1,
        float multiplier = 2f,
        int duration = 3,
        bool refreshDuration = true
    )
    {
        StackCount = stackCount;
        Multiplier = multiplier;
        Duration = duration;
        RefreshDuration = refreshDuration;
    }
}