using TMPro;
using UnityEngine;
public class SidenoteItem : MonoBehaviour
{
    public TextMeshPro text;
    public Transform bg;
    [SerializeField] string testStr = "";
    public float Width => (text.transform as RectTransform).sizeDelta.x + 0.26f;
    public float Height => text.preferredHeight;
    public void SetMsg(string msg)
    {
        text.text = msg;
        text.ForceMeshUpdate();
        Vector3 scale = transform.localScale;
        scale.y = Height;
        scale.x = Width;
        bg.localScale = scale;
    }
    private void OnValidate()
    {
        if (text != null && bg != null)
        {
            SetMsg(testStr);
        }
    }
}