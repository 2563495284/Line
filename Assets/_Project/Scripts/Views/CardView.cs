using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private TMP_Text title;
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
        if (!Interactions.Instance.PlayerCanHover()) return;

        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactions.Instance.PlayerCanHover()) return;

        Vector3 pos = new(transform.position.x, cardHoverYOffset, 0);
        CardViewHoverSystem.Instance.Show(Card, pos);
        wrapper.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;

        if (Card.ManualTargetEffect != null)
        {
            ManualTargetingSystem.Instance.StartTargeting(MouseUtils.GetMousePositionInWorldSpace(mousePositionZValue));
        }
        else
        {
            Interactions.Instance.PlayerIsDragging = true;
            wrapper.SetActive(true);
            CardViewHoverSystem.Instance.Hide();
            dragStartPosition = transform.position;
            dragStartRotation = transform.rotation;
            transform.SetPositionAndRotation(MouseUtils.GetMousePositionInWorldSpace(mousePositionZValue), Quaternion.Euler(0, 0, 0));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;

        if (Card.ManualTargetEffect == null)
        {
            PlayCardOrResetPosition();

            Interactions.Instance.PlayerIsDragging = false;
            return;
        }
        LineView target = ManualTargetingSystem.Instance.EndTargeting(MouseUtils.GetMousePositionInWorldSpace(mousePositionZValue));
        if (target == null)
        {
            return;
        }

        if (!MultiStockSystem.Instance.CanTradeStock(target.StockType, Card.TradeStockAmount))
        {
            Utils.ShakeCamera();
            return;
        }
        PlayCardGA playCardGA = new(Card, PlayerAttributeSystem.Instance.playerView, target);
        ActionSystem.Instance.Perform(playCardGA);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!Interactions.Instance.PlayerCanInteract()) return;
        if (Card.ManualTargetEffect != null) return;

        transform.position = MouseUtils.GetMousePositionInWorldSpace(mousePositionZValue);
    }

    private bool CanPlayCard()
    {
        if (!Physics.Raycast(transform.position, Vector3.forward, out _, mouseUpRaycastDistance, dropAreaLayer))
        {
            return false;
        }
        if (!PlayerAttributeSystem.Instance.HasEnoughMana(Card.Mana))
        {
            return false;
        }
        return true;
    }

    private void PlayCardOrResetPosition()
    {
        if (CanPlayCard())
        {
            PlayCardGA playCardGA = new(Card, PlayerAttributeSystem.Instance.playerView);
            ActionSystem.Instance.Perform(playCardGA);
        }
        else
        {
            Utils.ShakeCamera();
            transform.SetPositionAndRotation(dragStartPosition, dragStartRotation);
        }
    }
}