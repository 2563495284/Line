public interface ICardReceiver
{

}
/// <summary>
/// 一定有目标，如果是没有目标的释放，则会触发CancelSelectCardCMD
/// </summary>
public class ReleaseCardCMD : LevelCommand
{
    public CardModel card;
    public ReleaseCardCMD(CardModel card)
    {
        this.card = card;
    }
}