using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerAttrList : MonoBehaviour
{
    public GameObject playerAttrItemPrefab;
    public Transform attrItemFolder;
    public LayoutMark sidenoteLayout;
    public float spaceX = 0.5f;
    public float spaceY = 0.5f;
    public int row = 1;
    public int col = 1;
    public void SetData(List<PlayerAttrData> infos)
    {
        int cnt = attrItemFolder.childCount;
        for (int i = 0; i < infos.Count; i++)
        {
            if (i >= cnt)
            {
                GameObject item = playerAttrItemPrefab.OPGet(attrItemFolder);
                item.transform.SetParent(attrItemFolder);
                PlayerAttrItem attrItem = item.GetComponent<PlayerAttrItem>();
                cnt++;
                attrItem.SetData(infos[i]);
            }
            else
            {
                PlayerAttrItem attrItem = attrItemFolder.GetChild(i).GetComponent<PlayerAttrItem>();
                attrItem.SetData(infos[i]);
            }
        }
        for (int i = infos.Count; i < cnt; i++)
        {
            attrItemFolder.GetChild(i).gameObject.OPPush();
        }
        RefreshLayout();
    }
    private void RefreshLayout()
    {
        Vector3 originPos = new Vector3(-(col - 1) / 2 * spaceX, (row - 1) / 2 * spaceY);
        for (int i = 0; i < attrItemFolder.childCount; i++)
        {
            Transform item = attrItemFolder.GetChild(i);
            int r = i / col;
            int c = i % col;
            item.localPosition = originPos + new Vector3(c * spaceX, -r * spaceY);
        }
    }
}