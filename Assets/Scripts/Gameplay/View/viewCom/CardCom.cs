using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CardCom : MonoBehaviour, IIndexableElement<int>, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public int Id { get; private set; }
    public Transform wrapper;
    public const string SelectEvt = "SelectCard";
    public const string PreviewEvt = "PreviewCard";
    public const string ReleaseEvt = "ReleaseCard";
    public const string ExitEvt = "ExitCard";
    public GameObject select;
    public GameObject ready;
    public Vector2 colliderSize = new Vector2(4, 5f);
    public BoxCollider2D clider;
    public void ActiveHandle()
    {
        clider.enabled = true;
    }
    public void BanHandle()
    {
        clider.enabled = false;
    }
    public void SetData(int cardId)
    {
        Id = cardId;
    }
    private void Update()
    {
        clider.size = new Vector2(colliderSize.x * wrapper.localScale.x, colliderSize.y * wrapper.localScale.y);
    }
    public void SetOrder(int index)
    {
        GetComponent<SortingGroup>().sortingOrder = index;
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    public void SetCancel()
    {
        select.SetActive(true);
        ready.SetActive(false);
    }
    public void Preview()
    {
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }
    public void CancelPreview()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
    public void SetReady()
    {
        ready.SetActive(true);
        select.SetActive(false);
    }
    public void Reset()
    {
        ready.SetActive(false);
        select.SetActive(false);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        this.Send(SelectEvt, this);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.Send(PreviewEvt, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.Send(ExitEvt, this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.Send(ReleaseEvt, this);
    }

    public int GetKey()
    {
        return Id;
    }
}