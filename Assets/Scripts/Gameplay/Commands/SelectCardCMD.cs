using UnityEngine;

public class SelectCardCMD : LevelCommand
{
    public CardModel card;
    public Vector2 worldPos;
    public SelectCardCMD(CardModel card, Vector2 worldPos)
    {
        this.card = card;
        this.worldPos = worldPos;
    }
}