using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
[RequireComponent(typeof(BoxCollider2D))]
public class SpriteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    public float enterScale = 1.1f;
    public float normalScale = 1f;
    public float downScale = 0.9f;
    private float targetScale = 1;
    public float speed = 5f;
    public UnityEvent onClick = new();
    private bool isInMouse = false;
    private bool isInDown = false;
    private void RefreshScale()
    {
        if (isInDown)
            targetScale = downScale;
        else if (isInMouse)
            targetScale = enterScale;
        else
            targetScale = normalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isInDown = true;
        RefreshScale();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInMouse = true;
        RefreshScale();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInMouse = false;
        RefreshScale();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isInDown = false;
        RefreshScale();
        onClick.Invoke();
    }
    private void Update()
    {
        float curScale = transform.localScale.x;
        float diff = speed * (targetScale - curScale);
        float nextScale = curScale + diff;
        if ((curScale - targetScale) * (nextScale - targetScale) < 0)
            nextScale = targetScale;
        transform.localScale = new Vector3(nextScale, nextScale, 1);

    }
}