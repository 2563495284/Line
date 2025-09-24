public class ChangeMoneyCMD : LevelCommand
{
    public float Amount { get; set; }

    public ChangeMoneyCMD(float amount)
    {
        Amount = amount;
    }
}
