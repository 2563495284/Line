public class ReleaseCardCMD : LevelCommand
{
    public CardModel card;
    public ReleaseCardCMD(CardModel card)
    {
        this.card = card;
    }
}