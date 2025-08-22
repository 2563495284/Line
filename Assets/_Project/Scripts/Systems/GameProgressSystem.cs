using System;
using System.Collections;
using UnityEngine;

public enum GameOutcome
{
    None,
    Win,
    Lose
}

/// <summary>
/// 游戏进度系统：跟踪回合、目标金额与胜负判定
/// </summary>
public class GameProgressSystem : Singleton<GameProgressSystem>
{
    [Header("目标设置")]
    [SerializeField] private int targetRounds = 100;
    [SerializeField] private float targetTotalAsset = 3_000_000f;

    [Header("UI")]
    [SerializeField] private GoalProgressUI goalProgressUI;
    [SerializeField] private EndGameUI endGameUI;

    private int currentRound = 0;
    private GameOutcome outcome = GameOutcome.None;

    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<NextRoundTurnGA>(OnNextRoundPost, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<NextRoundTurnGA>(OnNextRoundPost, ReactionTiming.POST);
    }

    private void Start()
    {
        UpdateProgressUI();
    }

    private void OnNextRoundPost(NextRoundTurnGA action)
    {
        // 如果游戏已结束或系统组件被销毁，不执行任何操作
        if (outcome != GameOutcome.None || this == null || !gameObject.activeInHierarchy)
            return;

        currentRound++;
        UpdateProgressUI();

        if (currentRound >= targetRounds)
        {
            EvaluateOutcome();
        }
    }

    private void EvaluateOutcome()
    {
        if (MultiStockSystem.Instance == null) return;

        float totalAsset = MultiStockSystem.Instance.GetTotalAssetValue();
        bool isWin = totalAsset >= targetTotalAsset;
        outcome = isWin ? GameOutcome.Win : GameOutcome.Lose;

        // 检查 endGameUI 是否存在且未被销毁
        if (endGameUI != null && endGameUI.gameObject != null)
        {
            endGameUI.Show(outcome, totalAsset, targetTotalAsset, RestartGame);
        }
    }

    private void UpdateProgressUI()
    {
        // 检查所有必要的组件是否存在且未被销毁
        if (goalProgressUI == null || goalProgressUI.gameObject == null ||
            MultiStockSystem.Instance == null || this == null)
            return;

        float totalAsset = MultiStockSystem.Instance.GetTotalAssetValue();
        float progress = Mathf.Clamp01(totalAsset / targetTotalAsset);
        int daysLeft = Mathf.Max(0, targetRounds - currentRound);
        goalProgressUI.UpdateUI(totalAsset, targetTotalAsset, progress, daysLeft);
    }

    public void RestartGame()
    {
        // 停止所有协程和动画
        StopAllCoroutines();

        // 重置游戏状态
        currentRound = 0;
        outcome = GameOutcome.None;

        // 重置所有游戏系统而不是重新加载场景
        StartCoroutine(RestartGameSystems());
    }

    private IEnumerator RestartGameSystems()
    {
        // 隐藏结束UI
        if (endGameUI != null)
        {
            endGameUI.HideImmediate();
        }

        // 重置多股市系统
        if (MultiStockSystem.Instance != null)
        {
            MultiStockSystem.Instance.ResetSystem();
        }

        // 重置玩家属性系统
        if (PlayerAttributeSystem.Instance != null)
        {
            PlayerAttributeSystem.Instance.ResetSystem();
        }

        // 重置NPC系统
        if (NPCSystem.Instance != null)
        {
            NPCSystem.Instance.ResetSystem();
        }

        // 重置Mana系统
        if (ManaSystem.Instance != null)
        {
            ManaSystem.Instance.ResetSystem();
        }

        // 重置新闻系统
        if (NewsSystem.Instance != null)
        {
            NewsSystem.Instance.ClearAllNews();
        }

        // 等待一帧确保所有重置完成
        yield return null;

        // 重新初始化游戏
        InitializeGame();

        // 更新UI
        UpdateProgressUI();
    }

    private void InitializeGame()
    {
        // 使用MatchSetupSystem重新初始化游戏
        if (MatchSetupSystem.Instance != null)
        {
            MatchSetupSystem.Instance.InitializeGame();
        }
    }
}


