using System.Collections.Generic;
using UnityEngine;

public class StockAttrList : MonoBehaviour
{
    public Transform itemFolder;
    public GameObject attrItemPrefab;
    public float spaceX = 0.5f;
    public float spaceY = 0.5f;
    public int row = 1;
    public int col = 1;
    [SerializeField] int testCnt = 5;
    public void RefreshLayout()
    {
        int cnt = transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            transform.GetChild(i).position = GetPosByIdx(i);
        }
    }
    public void SetData(List<StockAttrData> infos)
    {
        int cnt = itemFolder.childCount;
        for (int i = 0; i < infos.Count; i++)
        {
            GameObject target;
            if (i >= cnt)
            {
                cnt++;
                target = attrItemPrefab.OPGet(itemFolder);
            }
            else
            {
                target = itemFolder.GetChild(i).gameObject;
            }
            target.GetComponent<StockAttrItem>().SetInfo(infos[i]);
        }
        for (int i = infos.Count; i < cnt; i++)
        {
            itemFolder.GetChild(i).gameObject.OPPush();
        }
        RefreshLayout();
    }
    public Vector3 GetPosByIdx(int index)
    {
        int realRow = Mathf.FloorToInt(index / col);
        int realCol = index % col;
        float x = -(col - 1f) / 2f * spaceX + realCol * spaceX;
        float y = (row - 1f) / 2 * spaceY - realRow * spaceY;
        return transform.position + new Vector3(x, y);
    }
}