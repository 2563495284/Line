using System.Collections;
/// <summary>
/// 直接改变指定股票的持股数量（无向效果）
/// </summary>
public class Effect_Holding : Effect
{

    public int stockId;
    public int val;

    public Effect_Holding(string[] args) : base(args)
    {
        stockId = int.Parse(args[0]);
        val = int.Parse(args[1]);
    }

    public override IEnumerator Run(IEffectEmitter from)
    {
        yield return ExeCMD(new ChangeHoldingCMD(val, stockId));
    }
}