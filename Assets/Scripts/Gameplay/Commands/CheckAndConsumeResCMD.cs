using System.Collections.Generic;
public class CheckAndConsumeResourceCMD : LevelCommand
{
    public List<ResourceCost> ResourceCosts { get; private set; }
    public List<Effect2> SuccessEffects { get; private set; }
    public CharacterView CharacterView { get; private set; }
    public LineView TargetLineView { get; private set; }

    public CheckAndConsumeResourceCMD(
        List<ResourceCost> resourceCosts,
        List<Effect2> successEffects,
        CharacterView characterView,
        LineView targetLineView)
    {
        ResourceCosts = resourceCosts;
        SuccessEffects = successEffects;
        CharacterView = characterView;
        TargetLineView = targetLineView;
    }
}
