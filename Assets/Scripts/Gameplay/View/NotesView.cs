public class NotesView : LevelView
{
    protected override void OnShow()
    {
        Register<ShowSidenoteArgs>(NotifyConst.ShowSidenote, OnShowSidenote);
        Register(NotifyConst.HideSidenote, OnHideSidenote);
    }
    protected override void OnHide()
    {
        Unregister<ShowSidenoteArgs>(NotifyConst.ShowSidenote, OnShowSidenote);
        Unregister(NotifyConst.HideSidenote, OnHideSidenote);
    }
    /// <summary>
    /// 展示词条旁注
    /// </summary>
    private void OnShowSidenote(ShowSidenoteArgs args)
    {

    }
    /// <summary>
    /// 关闭
    /// </summary>
    private void OnHideSidenote()
    {

    }
}