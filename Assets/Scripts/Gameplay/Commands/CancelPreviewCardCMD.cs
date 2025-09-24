public class CancelPreviewCardCMD : LevelCommand
{
    public CardModel card;
    public CancelPreviewCardCMD(CardModel card)
    {
        this.card = card;
    }
}