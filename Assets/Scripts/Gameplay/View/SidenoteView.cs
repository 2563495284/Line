using System.Collections.Generic;
using UnityEngine;
public class SidenoteLayoutInfo
{
    public Vector3 pos;
    public ELayoutDir layoutDir;
    public float heightLimit;
}
public class SidenoteView : LevelView
{
    public float spaceX = 0.2f;
    public float spaceY = 0.2f;
    public Transform noteItemsFolder;
    public GameObject noteItemPrefab;
    private SidenoteLayoutInfo layoutInfo;
    public override void OnEnter()
    {
        base.OnEnter();
        Register<ShowSidenoteArgs>(EventConst.ShowSidenote, OnShowSidenote);
        Register(EventConst.HideSidenote, OnHideSidenote);
    }
    public override void OnExit()
    {
        base.OnExit();
        Unregister<ShowSidenoteArgs>(EventConst.ShowSidenote, OnShowSidenote);
        Unregister(EventConst.HideSidenote, OnHideSidenote);
    }
    /// <summary>
    /// 展示词条旁注
    /// </summary>
    private void OnShowSidenote(ShowSidenoteArgs args)
    {
        noteItemsFolder.gameObject.SetActive(true);
        layoutInfo = args.layoutInfo;
        SetData(args.messages);
    }
    private void SetData(List<string> infos)
    {

        int cnt = noteItemsFolder.childCount;
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject target;
            if (i >= cnt)
            {
                cnt++;
                target = noteItemPrefab.OPGet(noteItemsFolder);
            }
            else
            {
                target = noteItemsFolder.GetChild(i).gameObject;
            }
            target.GetComponent<SidenoteItem>().SetMsg(infos[i]);
        }
        for (int i = infos.Count; i < cnt; i++)
        {
            noteItemsFolder.GetChild(i).gameObject.OPPush();
        }
        RefreshLayout();
    }
    /// <summary>
    /// 关闭
    /// </summary>
    private void OnHideSidenote()
    {
        noteItemsFolder.gameObject.SetActive(false);
    }
    private void RefreshLayout()
    {
        transform.position = layoutInfo.pos;
        float layoutY = 0;
        float layoutX = 0;
        for (int i = 0; i < noteItemsFolder.childCount; i++)
        {
            SidenoteItem item = noteItemsFolder.GetChild(i).GetComponent<SidenoteItem>();
            if (layoutInfo.layoutDir == ELayoutDir.LeftToRight)
            {
                item.transform.localPosition = new Vector3(layoutX + item.Width / 2, layoutY - item.Height / 2);
                layoutY = layoutY - item.Height - spaceY;
                if (layoutY < -layoutInfo.heightLimit)
                {
                    layoutY = 0;
                    layoutX += item.Width + spaceX;
                }
            }
            else
            {
                item.transform.localPosition = new Vector3(layoutX - item.Width / 2, layoutY - item.Height / 2);
                layoutY = layoutY - item.Height - spaceY;
                if (layoutY < -layoutInfo.heightLimit)
                {
                    layoutY = 0;
                    layoutX -= item.Width + spaceX;
                }
            }
        }
    }
}