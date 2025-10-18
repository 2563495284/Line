using System.Collections;
using UnityEngine;

public class GameView : LevelView
{
    public ClickModal clickModal;
    public override void OnEnter()
    {
        base.OnEnter();
        Bind(EventConst.EnterLevelView, EnterLevelViewPerformer);
    }
    public override void OnExit()
    {
        base.OnExit();
        Unbind(EventConst.EnterLevelView, EnterLevelViewPerformer);
    }
    private IEnumerator EnterLevelViewPerformer()
    {

        yield return null;
    }
}