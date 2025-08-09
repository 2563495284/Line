using System.Collections.Generic;

[System.Serializable]
public abstract class Effect
{
    protected CharacterView characterView;
    public abstract GameAction GetGameAction();

    public void SetCharacterView(CharacterView characterView)
    {
        this.characterView = characterView;
    }

}