using System.Collections;
//变更能量
public class Effect_Mana : Effect
{
    public int val;

    public Effect_Mana(string[] args) : base(args)
    {
        val = int.Parse(args[0]);
    }

    public override IEnumerator Run(IEffectEmitter from)
    {
        yield return ExeCMD(new ChangeManaCMD(val));
    }
}