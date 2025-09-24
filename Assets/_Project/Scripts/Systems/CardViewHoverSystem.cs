using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardViewHoverSystem : SingletonCom<CardViewHoverSystem>
{
    [SerializeField] private CardView cardViewHover;
    [SerializeField] private AttributeTooltipDisplay attributeTooltipDisplay;

    public void Show(Card card, Vector3 position)
    {
        // 更新卡牌描述以获取最新的属性数值
        card.UpdateDescription();

        // 显示卡牌悬停视图
        cardViewHover.gameObject.SetActive(true);
        cardViewHover.Setup(card);
        cardViewHover.transform.position = position;

        // 显示属性提示框
        ShowAttributeTooltip(card, position);
    }

    public void Hide()
    {
        cardViewHover.gameObject.SetActive(false);
        if (attributeTooltipDisplay != null)
        {
            attributeTooltipDisplay.Hide();
        }
    }

    /// <summary>
    /// 显示属性提示框
    /// </summary>
    private void ShowAttributeTooltip(Card card, Vector3 cardPosition)
    {
        if (attributeTooltipDisplay == null) return;

        // 获取卡牌引用的属性
        var referencedAttributes = card.ReferencedAttributes;
        if (referencedAttributes == null || referencedAttributes.Count == 0) return;

        // 直接传递卡牌位置，让AttributeTooltipDisplay处理偏移
        // 显示属性提示框，高亮所有引用的属性
        attributeTooltipDisplay.Show(referencedAttributes, referencedAttributes, cardPosition);
    }
}