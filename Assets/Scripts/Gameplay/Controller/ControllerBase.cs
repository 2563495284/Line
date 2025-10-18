using System.Collections;
using UnityEngine;

public class ControllBase
{
    public LevelRoot Root { get; private set; }
    public ControllBase(LevelRoot root)
    {
        Root = root;
    }
    protected Coroutine InvokeAsync(IEnumerator method, string tag = "")
    {
        if (UIManager.Ins.console.isTrackCoroutine)
            return Root.StartTrackedCoroutine(method, tag);
        else
            return Root.StartCoroutine(method);
    }
    public virtual void OnEnter()
    {

    }
    public virtual void OnExit()
    {
        Root.StopAllCoroutines();
    }
}