public class EnemyPlayCardCMD : LevelCommand
{
    public readonly CardModel card;
    public EnemyModel enemy;
    public EnemyPlayCardCMD(CardModel card, EnemyModel enemy)
    {
        this.enemy = enemy;
        this.card = card;
    }
}