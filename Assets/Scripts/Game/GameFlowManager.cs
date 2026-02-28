using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    InGame
}

/// <summary>
/// 游戏流程管理器 —— 负责场景切换与当前存档的持有。
/// 跨场景存活 (DontDestroyOnLoad)，是游戏全局状态的入口。
/// </summary>
public class GameFlowManager : PersistentSingleton<GameFlowManager>
{
    private const string MAIN_MENU_SCENE = "SampleScene";
    private const string GAME_SCENE = "GameScene";

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    /// <summary>当前局持有的全局存档，InGame 状态下始终不为 null</summary>
    public GlobalSaveData CurrentSave { get; private set; }

    // ── 主菜单流程 ─────────────────────────────────────────────────────────────

    /// <summary>新游戏：覆盖已有存档（如有），进入游戏场景</summary>
    public void StartNewGame()
    {
        CurrentSave = SaveManager.Instance.CreateNewSave();
        RandomManager.Instance.Initialize(CurrentSave.randomSeed);
        CurrentState = GameState.InGame;
        SceneManager.LoadScene(GAME_SCENE);
        Debug.Log("[GameFlowManager] 新游戏开始");
    }

    /// <summary>读档：加载已有存档并进入游戏场景</summary>
    public void LoadGame()
    {
        var save = SaveManager.Instance.Load();
        if (save == null)
        {
            Debug.LogError("[GameFlowManager] 读档失败，无法进入游戏");
            return;
        }
        CurrentSave = save;
        RandomManager.Instance.Initialize(CurrentSave.randomSeed);
        CurrentState = GameState.InGame;
        SceneManager.LoadScene(GAME_SCENE);
        Debug.Log("[GameFlowManager] 读档成功，进入游戏");
    }

    // ── 游戏内流程 ─────────────────────────────────────────────────────────────

    /// <summary>手动保存当前存档</summary>
    public void SaveCurrentGame()
    {
        if (CurrentSave == null)
        {
            Debug.LogWarning("[GameFlowManager] 当前没有存档可保存");
            return;
        }
        SaveManager.Instance.Save(CurrentSave);
    }

    /// <summary>自动存档后返回主菜单</summary>
    public void ReturnToMainMenu()
    {
        SaveCurrentGame();
        CurrentState = GameState.MainMenu;
        SceneManager.LoadScene(MAIN_MENU_SCENE);
        Debug.Log("[GameFlowManager] 返回主菜单");
    }

    /// <summary>删除存档并返回主菜单（用于游戏内删档）</summary>
    public void DeleteSaveAndReturnToMenu()
    {
        CurrentSave = null;
        SaveManager.Instance.Delete();
        CurrentState = GameState.MainMenu;
        SceneManager.LoadScene(MAIN_MENU_SCENE);
        Debug.Log("[GameFlowManager] 存档已删除，返回主菜单");
    }
}
