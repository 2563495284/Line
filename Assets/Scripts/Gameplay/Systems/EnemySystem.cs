using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemySystem : LevelSystem
{
    private List<EnemyModel> enemies = new();

    public EnemySystem(LevelController ctrl) : base(ctrl)
    {
    }
    public override void EnableSystem()
    {
        Ctrl.BindProcessor<ChangeStrategyCMD>(ChangeStrategyProcessor);
        Ctrl.AddRection<NextRoundTurnCMD>(NextRoundTurnPreReaction, ReactionTiming.PRE);
    }
    public override void DisableSystem()
    {
        Ctrl.UnbindPerformer<ChangeStrategyCMD>();
        Ctrl.RemoveRection<NextRoundTurnCMD>(NextRoundTurnPreReaction, ReactionTiming.PRE);
    }


    [TraceableCoroutine("EnemyChangeStrategy")]
    private IEnumerator ChangeStrategyProcessor(ChangeStrategyCMD changeStrategyCMD)
    {
        changeStrategyCMD.Target.curStrategy = changeStrategyCMD.StrategyType;
        yield return null;
    }
    /// <summary>
    /// 执行NPC动作的核心逻辑
    /// </summary>
    private void NextRoundTurnPreReaction(NextRoundTurnCMD cmd)
    {

        foreach (var enemy in enemies)
        {
            if (enemy.handCards.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, enemy.handCards.Count);
                CardModel cardToPlay = enemy.handCards[randomIndex];
                EnemyPlayCardCMD playCardCMD = new(cardToPlay, enemy);
                Ctrl.ExeCMD(playCardCMD);
            }
            else
            {
                EnemyDrawCardsCMD drawCardsCMD = new(enemy.Cfg.maxHandSize, enemy);
                Ctrl.ExeCMD(drawCardsCMD);
            }
        }
    }

}