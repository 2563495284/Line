using Unity.VisualScripting;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    private void Awake()
    {
        GameObject objectPool = new("ObjectPoolRoot");
        objectPool.AddComponent<OP>();
        objectPool.transform.SetParent(transform);
    }
    private void Start()
    {
        GM.Ins.EnterGame();
    }
    private void Update()
    {
        TM.OnUpdate();
    }
}