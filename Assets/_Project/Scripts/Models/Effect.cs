using System.Collections.Generic;

[System.Serializable]
public abstract class Effect
{
    protected CharacterView characterView;
    protected LineView targetLineView;
    public abstract GameAction GetGameAction();

    public void SetTargetLineView(LineView targetLineView)
    {
        this.targetLineView = targetLineView;
    }

    public void SetCharacterView(CharacterView characterView)
    {
        this.characterView = characterView;
    }
}