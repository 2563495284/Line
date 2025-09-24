public class EnemyDrawCardsCMD : LevelCommand
{
    public EnemyModel enemy { get; set; }
    public int Amount { get; set; }

    public EnemyDrawCardsCMD(int amount, EnemyModel enemy)
    {
        Amount = amount;
        this.enemy = enemy;
    }
}