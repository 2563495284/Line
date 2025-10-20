using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardSystem : LevelSystem
{
    public override void OnEnter()
    {
        base.OnEnter();
        BindProcessor<DrawActionCardsCMD>(DrawActionCardsProcessor);
        BindProcessor<DiscardAllActionCardsCMD>(DiscardAllActionCardsProcessor);
        BindProcessor<ReleaseCardCMD>(ReleaseCardProcessor);
        BindProcessor<DrawTradeCardsCMD>(DrawTradeCardsProcessor);
        BindProcessor<DiscardAllTradeCardsCMD>(DiscardAllTradeCardsProcessor);
    }
    public override void OnExit()
    {
        base.OnExit();
        UnbindProcessor<DrawActionCardsCMD>();
        UnbindProcessor<DiscardAllActionCardsCMD>();
        UnbindProcessor<ReleaseCardCMD>();
        UnbindProcessor<DrawTradeCardsCMD>();
        UnbindProcessor<DiscardAllTradeCardsCMD>();
    }
    [TagEnumerator("DiscardAllActionCards")]
    private IEnumerator DiscardAllActionCardsProcessor(DiscardAllActionCardsCMD cmd)
    {
        List<int> hands = new(Model.handCards_action);
        Model.handCards_action.ForEach(e => Model.discardCards_action.Add(e));
        Model.handCards_action.Clear();
        yield return Perform(EventConst.DiscardActionCards, new DiscardActionCardsArgs(hands));
    }
    [TagEnumerator("DrawTradeCards")]
    private IEnumerator DrawTradeCardsProcessor(DrawTradeCardsCMD cmd)
    {
        int num = Math.Min(cmd.num, Model.drawCards_trade.Count);
        List<int> res = Model.drawCards_trade.WeiRandMul(Model.drawCards_trade.Select(e => 1f).ToList(), num, false);
        Model.handCards_trade = Model.handCards_trade.Concat(res).ToList();
        yield return Perform(EventConst.DrawTradeCards, new DrawTradeCardsArgs(res));
    }
    [TagEnumerator("DiscardTradeCards")]
    private IEnumerator DiscardAllTradeCardsProcessor(DiscardAllTradeCardsCMD cmd)
    {
        List<int> hands = new(Model.handCards_trade);
        Model.handCards_trade.Clear();
        yield return Perform(EventConst.DiscardTradeCards, new DiscardTradeCardsArgs(hands));

    }
    [TagEnumerator("DrawActionCards")]
    private IEnumerator DrawActionCardsProcessor(DrawActionCardsCMD cmd)
    {
        int num = cmd.num;
        while (num > 0)
        {
            List<int> drawed = new();
            if (Model.drawCards_action.Count <= 0)
            {
                if (Model.discardCards_action.Count == 0)
                    break;
                Model.drawCards_action = new(Model.discardCards_action);
                Model.drawCards_action.Shuffle();
                Model.discardCards_action.Clear();
                yield return Perform(EventConst.RefillCards, new RefillCardsArgs(new List<int>(Model.drawCards_action)));
            }
            int allCnt = Model.drawCards_action.Count;
            int realDraw = Math.Min(allCnt, num);
            num -= realDraw;
            while (realDraw > 0)
            {
                int drawedId = Model.drawCards_action.Draw();
                Model.handCards_action.Add(drawedId);
                drawed.Add(drawedId);
                realDraw--;
            }

            yield return Perform(EventConst.DrawActionCards, new DrawActionCardsArgs(drawed));
        }
    }
    [TagEnumerator("ReleaseCard")]
    private IEnumerator ReleaseCardProcessor(ReleaseCardCMD cmd)
    {
        //TODO 卡牌效果
        Model.handCards_action.Remove(cmd.cardId);
        Model.discardCards_action.Add(cmd.cardId);
        yield return Perform(EventConst.CardEffectStart, new CardEffectStartArgs(cmd.cardId));
        yield return Perform(EventConst.DiscardActionCards, new DiscardActionCardsArgs(new List<int>() { cmd.cardId }));
    }
}