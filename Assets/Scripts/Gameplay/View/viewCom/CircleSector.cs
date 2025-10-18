using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CircleSector : MonoBehaviour
{
    public int stockId = 1;
    public SpriteRenderer sector1;
    string SidenoteMsg => "<color=#fef471>公司实力:{stock" + stockId + ".factor1}</color>\n<color=#6565fe>市场敏感度:{stock" + stockId + ".factor2}</color>\n<color=#56fe97>行业冲击影响:{stock" + stockId + ".factor3}</color>";
    public SpriteRenderer sector2;
    public SpriteRenderer sector3;
    public float val1 = 0.3f;
    public float val2 = 0.5f;
    public float val3 = 0.2f;
    public Color color1 = Color.yellow;
    public Color color2 = Color.blue;
    public Color color3 = Color.cyan;
    public TextMeshPro text1;
    public TextMeshPro text2;
    public TextMeshPro text3;
    void Awake()
    {
        GetComponent<SideNoteTarget>().messages = new List<string> { SidenoteMsg };
    }

    public void Refresh()
    {
        if (sector1 == null || sector2 == null || sector3 == null)
            return;
        float total = val1 + val2 + val3;
        float mul = 360 / total;
        sector1.material.SetFloat("_StartAngle", 0);
        sector1.material.SetFloat("_EndAngle", val1 * mul);
        sector2.material.SetFloat("_StartAngle", val1 * mul);
        sector2.material.SetFloat("_EndAngle", (val1 + val2) * mul);
        sector3.material.SetFloat("_StartAngle", (val1 + val2) * mul);
        sector3.material.SetFloat("_EndAngle", (val1 + val2 + val3) * mul);
        sector1.material.SetColor("_SectorColor", color1);
        sector2.material.SetColor("_SectorColor", color2);
        sector3.material.SetColor("_SectorColor", color3);

        if (text1 != null && text2 != null && text3 != null)
        {
            float textDist = transform.localScale.x * 2 / 6;
            float textAngle1 = val1 / 2 * mul;
            float textAngle2 = (val1 + val2 / 2) * mul;
            float textAngle3 = (val1 + val2 + val3 / 2) * mul;
            text1.transform.position = transform.position + GetVectorFromAngle(textAngle1) * textDist;
            text2.transform.position = transform.position + GetVectorFromAngle(textAngle2) * textDist;
            text3.transform.position = transform.position + GetVectorFromAngle(textAngle3) * textDist;
            text1.text = $"{(val1 / total * 100).ToString("F0")}%";
            text2.text = $"{(val2 / total * 100).ToString("F0")}%";
            text3.text = $"{(val3 / total * 100).ToString("F0")}%";
        }
    }
    public Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}