using System.Collections;
using UnityEngine;

public class ControllBase
{
    public LevelRoot Root { get; private set; }
    public ControllBase(LevelRoot root)
    {
        Root = root;
    }
    protected Coroutine InvokeAsync(IEnumerator method)
    {
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