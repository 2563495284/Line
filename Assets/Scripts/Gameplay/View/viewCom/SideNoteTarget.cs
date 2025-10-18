using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(BoxCollider2D))]
public class SideNoteTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LayoutMark sidenoteMark;
    public List<string> messages = new();
    public void OnPointerEnter(PointerEventData eventData)
    {
        GM.Ins.Level.Notify(EventConst.ShowSidenote, new ShowSidenoteArgs { messages = messages, layoutInfo = sidenoteMark.GetSidenoteLayoutInfo() });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.Ins.Level.Notify(EventConst.HideSidenote);
    }
}