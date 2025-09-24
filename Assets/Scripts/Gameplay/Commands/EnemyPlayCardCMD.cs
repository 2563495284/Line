public class EnemyPlayCardCMD : LevelCommand
{
    public readonly CardModel card;
    public EnemyPlayCardCMD(CardModel card)
    {
        this.card = card;
    }
}