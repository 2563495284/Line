using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SuggestionBox : MonoBehaviour
{
    public List<string> Matches { get; private set; } = new();
    public TextMeshProUGUI text;
    public int maxMatchNum = 8;
    public float paddingOffset = 16;
    public bool onlyTips = false;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Clear()
    {
        text.text = "";
        onlyTips = false;
        Matches.Clear();
    }
    public void SetList(List<string> matches, bool onlyTips = false)
    {
        this.onlyTips = onlyTips;
        Matches = matches.Take(8).ToList();
        string str = "";
        if (Matches.Count > 0)
        {
            for (int i = 0; i < Matches.Count; i++)
            {
                str += Matches[i];
                if (i != Matches.Count - 1)
                    str += "\n";
            }
        }
        else
        {
            str = "no matches";
        }
        text.text = str;
        text.ForceMeshUpdate();

        // TMP的preferredHeight已经包含了所有文本行的总高度
        float textHeight = text.preferredHeight + paddingOffset;
        RectTransform rt = transform as RectTransform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, textHeight);
    }
}