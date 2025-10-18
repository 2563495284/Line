using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public Button button;
    void Awake()
    {
        button.onClick.AddListener(StartLevel);
    }
    private void StartLevel()
    {
        GM.Ins.StartLevel();
    }
}
