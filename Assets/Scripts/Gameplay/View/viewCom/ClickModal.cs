using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickModal : MonoBehaviour, IPointerDownHandler
{
    public const string OnClickModal = "OnClickModal";
    public void OnPointerDown(PointerEventData eventData)
    {
        this.Send(OnClickModal);
    }
}
