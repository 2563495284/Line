using GameConfig;
using Unity.VisualScripting;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    private void Awake()
    {
        GameObject objectPool = new("ObjectPoolRoot");
        objectPool.AddComponent<OP>();
        objectPool.transform.SetParent(transform);
        uiManager.console.AddLog("Parse Config", LogMsgType.Start);
        Config.Init(ResPath.configs.Config);
        uiManager.console.AddLog("Parse Config", LogMsgType.End);
        uiManager.console.AddLog("Load dynamic class", LogMsgType.Start);
        LoadManager.Ins.LoadAllDynamicClass();
        uiManager.console.AddLog("Load dynamic class", LogMsgType.End);
        uiManager.console.AddLog("Init Config", LogMsgType.Start);
        GlobalConfig.Ins.Init();
        uiManager.console.AddLog("Init Config", LogMsgType.End);
    }
    private void Start()
    {
        GM.Ins.EnterGame();
    }
    private void Update()
    {
        TM.OnUpdate();
        GM.Ins.OnUpdate();
    }
}