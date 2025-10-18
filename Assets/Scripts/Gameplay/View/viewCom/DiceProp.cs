using UnityEngine;
[RequireComponent(typeof(SideNoteTarget))]
public class DiceProp : MonoBehaviour
{
    public int DiceId { get; private set; }
    public void SetData(int diceId)
    {
        DiceId = diceId;
    }
}