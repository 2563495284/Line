public class PlayCardCMD : LevelCommand
{
    public CardModel card;
    public IEffectReceiver receiver;

    public PlayCardCMD(CardModel card, IEffectReceiver receiver = null)
    {
        this.card = card;
        this.receiver = receiver;
    }
}