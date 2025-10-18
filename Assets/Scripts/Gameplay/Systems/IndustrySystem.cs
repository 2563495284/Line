using System.Collections;

public class IndustrySystem : LevelSystem
{

    private float GetIndustryPower()
    {
        return MUtils.RandF(-1f, 1f);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        BindProcessor<UpdateIndustryPowerCMD>(UpdateIndustryPowerProcessor);
    }
    public override void OnExit()
    {
        base.OnExit();
        UnbindProcessor<UpdateIndustryPowerCMD>();

    }

    private IEnumerator UpdateIndustryPowerProcessor(UpdateIndustryPowerCMD cmd)
    {
        yield return AwaitCMD(new ApplyIndustryPowerCMD(GetIndustryPower()));
    }
}