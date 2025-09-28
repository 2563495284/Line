using System.Collections;

public class Effect_Money : Effect
{
    public int val;

    public Effect_Money(string[] args) : base(args)
    {
        val = int.Parse(args[0]);
    }

    public override IEnumerator Run(IEffectEmitter from)
    {
        yield return ExeCMD(new ChangeMoneyCMD(val));
    }
}