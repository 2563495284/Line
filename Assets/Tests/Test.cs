using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject prefab;
    public Queue<GameObject> activedGO = new();
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            activedGO.Enqueue(prefab.OPGet(transform));
        }
        else if (Input.GetMouseButtonDown(1) && activedGO.Count > 0)
        {
            activedGO.Dequeue().OPPush();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            prefab.OPClear();
        }
        // else if (Input.GetKeyDown(KeyCode.A))
        // {
        //     Debug.Log("tag cnt: " + OP.m_GoTag.Count);
        //     Debug.Log("id cnt: " + OP.m_IDCnt.Count);
        //     Debug.Log("pool cnt: " + OP.m_Pool.Count);
        // }
    }
}
