public class ChangeManaCMD : LevelCommand
{
    public int Amount { get; set; }

    public ChangeManaCMD(int amount)
    {
        Amount = amount;
    }
}