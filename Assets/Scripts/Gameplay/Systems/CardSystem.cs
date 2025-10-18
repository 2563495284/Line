using System;
using System.Collections;
using System.Collections.Generic;

public class CardSystem : LevelSystem
{
    public override void OnEnter()
    {
        base.OnEnter();
        BindProcessor<DrawActionCardsCMD>(DrawActionCardsProcessor);
        BindProcessor<DiscardAllCardsCMD>(DiscardAllCardsProcessor);
        BindProcessor<ReleaseCardCMD>(ReleaseCardProcessor);
    }
    public override void OnExit()
    {
        base.OnExit();
        UnbindProcessor<DrawActionCardsCMD>();
        UnbindProcessor<DiscardAllCardsCMD>();
        UnbindProcessor<ReleaseCardCMD>();
    }
    private IEnumerator DiscardAllCardsProcessor(DiscardAllCardsCMD cmd)
    {
        List<int> hands = new(Model.handCards_action);
        Model.handCards_action.ForEach(e => Model.discardCards_action.Add(e));
        Model.handCards_action.Clear();
        yield return Perform(EventConst.DiscardCards, new DiscardCardsArgs(hands));
    }
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

            yield return Perform(EventConst.DrawCards, new DrawCardsArgs(drawed));
        }
    }
    private IEnumerator ReleaseCardProcessor(ReleaseCardCMD cmd)
    {
        //TODO 卡牌效果
        Model.handCards_action.Remove(cmd.cardId);
        Model.discardCards_action.Add(cmd.cardId);
        yield return Perform(EventConst.CardEffectStart, new CardEffectStartArgs(cmd.cardId));
        yield return Perform(EventConst.DiscardCards, new DiscardCardsArgs(new List<int>() { cmd.cardId }));
    }
}