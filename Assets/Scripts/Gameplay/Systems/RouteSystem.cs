using System.Collections;

public class RouteSystem : LevelSystem
{
    public override void OnEnter()
    {
        base.OnEnter();
        BindProcessor<FinishRouteCMD>(FinishRouteProcessor);
        BindProcessor<RouteRunCMD>(RouteProcessor);
    }
    public override void OnExit()
    {
        base.OnExit();
        UnbindProcessor<FinishRouteCMD>();
        UnbindProcessor<RouteRunCMD>();
    }

    bool finishRoute = false;
    private IEnumerator FinishRouteProcessor(FinishRouteCMD cmd)
    {
        finishRoute = true;
        yield break;
    }
    private IEnumerator RouteProcessor(RouteRunCMD cmd)
    {
        yield return Perform(EventConst.EnterRouteView);
        finishRoute = false;
        while (!finishRoute)
            yield return null;

    }
}