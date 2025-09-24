using UnityEngine;

public class DragCardCMD : LevelCommand
{
    public CardModel card;
    public Vector2 worldPos;
    public DragCardCMD(CardModel card, Vector2 worldPos)
    {
        this.card = card;
        this.worldPos = worldPos;
    }
}