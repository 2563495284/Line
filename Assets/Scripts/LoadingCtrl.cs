using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tips;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressLbl;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void SetProgress(float progress)
    {
        progressBar.fillAmount = progress;
        progressLbl.text = $"{(progress * 100).ToString("F1")}%";
    }
    public void SetTips(string str)
    {
        tips.text = str;
    }
}