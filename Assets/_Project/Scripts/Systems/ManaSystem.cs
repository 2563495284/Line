using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaSystem : SingletonCom<ManaSystem>
{
    [SerializeField] private ManaUI manaUI;

    [Header("能量系统")]
    [SerializeField] private int baseEnergyPerTurn = 3;
    [SerializeField] private int currentMana = 3;
    [SerializeField] private int maxMana = 999;
    [SerializeField] private int savedMana = 0; // 耐心属性保存的能量

    protected override void Awake()
    {
        base.Awake();
        currentMana = baseEnergyPerTurn;
        manaUI.UpdateManaText(currentMana);
    }

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ChangeManaGA>(ChangeManaPerformer);
        ActionSystem.AttachPerformer<RefillManaGA>(RefillManaPerformer);

        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ChangeManaGA>();
        ActionSystem.DetachPerformer<RefillManaGA>();
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(NextRoundTurnPostReaction, ReactionTiming.POST);
    }

    public bool HasEnoughMana(int mana)
    {
        return currentMana >= mana;
    }

    private IEnumerator ChangeManaPerformer(ChangeManaGA changeManaGA)
    {
        currentMana += changeManaGA.Amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }
    private IEnumerator RefillManaPerformer(RefillManaGA refillManaGA)
    {
        currentMana = GetTotalEnergyPerTurn();
        savedMana = 0;
        manaUI.UpdateManaText(currentMana);
        yield return null;
    }
    private void NextRoundTurnPreReaction(NextRoundTurnGA nextRoundTurnGA)
    {
        //存储能量
        int saveableAmount = GetSaveableEnergy();
        int energyToSave = Mathf.Min(currentMana, saveableAmount);
        savedMana = energyToSave;
        currentMana = savedMana;
        manaUI.UpdateManaText(currentMana);
    }
    private void NextRoundTurnPostReaction(NextRoundTurnGA nextRoundTurnGA)
    {
        //恢复能量
        RefillManaGA refillManaGA = new();
        ActionSystem.Ins.AddReaction(refillManaGA);
    }
    /// <summary>
    /// 获取每回合能量恢复数
    /// </summary>
    public int GetEnergyPerTurn()
    {
        int wisdomBonus = (int)PlayerAttributeSystem.Ins.GetAttributeValue(EAttrType.Wisdom);
        return baseEnergyPerTurn + wisdomBonus;
    }

    /// <summary>
    /// 获取总能量上限（包括保存的能量）
    /// </summary>
    public int GetTotalEnergyPerTurn()
    {
        return GetEnergyPerTurn() + savedMana;
    }

    /// <summary>
    /// 获取可保存的能量数量
    /// </summary>
    public int GetSaveableEnergy()
    {
        return 0;
    }

    /// <summary>
    /// 重置Mana系统到初始状态
    /// </summary>
    public void ResetSystem()
    {
        currentMana = baseEnergyPerTurn;
        savedMana = 0;

        if (manaUI != null)
        {
            manaUI.UpdateManaText(currentMana);
        }
    }
}