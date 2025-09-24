public class ChangeStrategyCMD : LevelCommand
{
    public EnemyModel Target { get; set; }
    public EStrategyType StrategyType { get; set; }

    public ChangeStrategyCMD(EnemyModel enemy, EStrategyType strategyType)
    {
        Target = enemy;
        StrategyType = strategyType;
    }
}