using UnityEngine;
using UnityEngine.EventSystems;

public class GoodsItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public const string ConfirmBuy = "ConfirmBuy";
    public GameObject confirmTips;
    private bool isInGoods = false;
    private bool isInConfirmTips = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isInConfirmTips)
        {
            this.Send(ConfirmBuy);
            ExitConfirm();
        }
        else
        {
            EnterConfirm();
        }
    }
    private void EnterConfirm()
    {
        isInConfirmTips = true;
        confirmTips.SetActive(true);
    }
    private void ExitConfirm()
    {
        isInConfirmTips = false;
        confirmTips.SetActive(false);
    }
    private void Update()
    {
        if (isInConfirmTips && Input.GetMouseButtonDown(0) && !isInGoods)
            ExitConfirm();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isInGoods = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInGoods = false;
    }
}