using System.Collections;
/// <summary>
/// 立即抽取两张卡牌
/// </summary>
public class Effect_Draw : Effect
{
    private int drawCnt;
    public Effect_Draw(string[] args) : base(args)
    {
        drawCnt = int.Parse(args[0]);
    }

    public override IEnumerator Run(IEffectEmitter from)
    {
        yield return ExeCMD(new DrawCardsCMD(drawCnt));
    }
}