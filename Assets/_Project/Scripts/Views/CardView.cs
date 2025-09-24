using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private TMP_Text description;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask dropAreaLayer;

    public Card Card { get; private set; }

    private Vector3 dragStartPosition;
    private Quaternion dragStartRotation;

    private readonly float cardHoverYOffset = -2f;
    private readonly float mousePositionZValue = -1f;
    private readonly float mouseUpRaycastDistance = 10f;

    public void Setup(Card card)
    {
        Card = card;
        title.text = Card.Title;
        mana.text = Card.Mana.ToString();
        // 使用富文本描述以显示高亮效果
        description.text = Card.RichTextDescription ?? Card.Description;

        imageSR.sprite = Card.Image;
    }
    public void UpdateDescription()
    {
        description.text = Card.RichTextDescription ?? Card.Description;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactions.Ins.PlayerCanHover()) return;

        CardViewHoverSystem.Ins.Hide();
        wrapper.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactions.Ins.PlayerCanHover()) return;

        Vector3 pos = new(transform.position.x, cardHoverYOffset, 0);
        CardViewHoverSystem.Ins.Show(Card, pos);
        wrapper.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactions.Ins.PlayerCanInteract()) return;

        if (Card.ManualTargetEffect != null)
        {
            ManualTargetingSystem.Ins.StartTargeting(MouseUtils.GetMouseWp(mousePositionZValue));
        }
        else
        {
            Interactions.Ins.PlayerIsDragging = true;
            wrapper.SetActive(true);
            CardViewHoverSystem.Ins.Hide();
            dragStartPosition = transform.position;
            dragStartRotation = transform.rotation;
            transform.SetPositionAndRotation(MouseUtils.GetMouseWp(mousePositionZValue), Quaternion.Euler(0, 0, 0));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactions.Ins.PlayerCanInteract()) return;

        if (Card.ManualTargetEffect == null)
        {
            PlayCardOrResetPosition();

            Interactions.Ins.PlayerIsDragging = false;
            return;
        }
        LineView target = ManualTargetingSystem.Ins.EndTargeting(MouseUtils.GetMouseWp(mousePositionZValue));
        if (target == null)
        {
            return;
        }
        if (!ManaSystem.Ins.HasEnoughMana(Card.Mana))
        {
            TipsSystem.Ins.ShowTip("能量不足");
            return;
        }
        PlayCardGA playCardGA = new(Card, PlayerAttributeSystem.Ins.playerView, target);
        ActionSystem.Ins.Perform(playCardGA);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!Interactions.Ins.PlayerCanInteract()) return;
        if (Card.ManualTargetEffect != null) return;

        transform.position = MouseUtils.GetMouseWp(mousePositionZValue);
    }

    private bool CanPlayCard()
    {
        if (!Physics.Raycast(transform.position, Vector3.forward, out _, mouseUpRaycastDistance, dropAreaLayer))
        {
            return false;
        }
        if (!ManaSystem.Ins.HasEnoughMana(Card.Mana))
        {
            TipsSystem.Ins.ShowTip("能量不足");
            return false;
        }
        return true;
    }

    private void PlayCardOrResetPosition()
    {
        if (CanPlayCard())
        {
            PlayCardGA playCardGA = new(Card, PlayerAttributeSystem.Ins.playerView);
            ActionSystem.Ins.Perform(playCardGA);
        }
        else
        {
            Utils.ShakeCamera();
            transform.SetPositionAndRotation(dragStartPosition, dragStartRotation);
        }
    }
}