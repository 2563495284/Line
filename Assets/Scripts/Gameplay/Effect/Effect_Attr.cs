
using System.Collections;
using GameConfig;

//type#val
//变更属性
public class Effect_Attr : Effect
{
    public AttrType type = 0;
    public float val = 0;

    public Effect_Attr(string[] args) : base(args)
    {
        type = (AttrType)int.Parse(args[0]);
        val = float.Parse(args[1]);
    }

    public override IEnumerator Run(IEffectEmitter from)
    {
        yield return ExeCMD(new ChangeAttributeCMD(type, val));
    }
}