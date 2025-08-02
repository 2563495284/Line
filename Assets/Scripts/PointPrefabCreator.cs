using UnityEngine;
using UnityEditor;
using TMPro;

public class PointPrefabCreator : MonoBehaviour
{
    [MenuItem("Tools/Create Point Prefab")]
    public static void CreatePointPrefab()
    {
        // 创建主GameObject
        GameObject pointObj = new GameObject("Point");

        // 添加Transform组件
        Transform transform = pointObj.transform;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * 0.2f;

        // 添加SpriteRenderer
        SpriteRenderer spriteRenderer = pointObj.AddComponent<SpriteRenderer>();

        // 创建圆形纹理
        Texture2D texture = CreateCircleTexture();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = Color.red;
        spriteRenderer.sortingOrder = 5;

        // 添加BoxCollider2D用于鼠标检测
        BoxCollider2D collider = pointObj.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 0.5f;

        // 添加Point脚本
        Point pointScript = pointObj.AddComponent<Point>();

        // 创建价格标签子对象
        GameObject labelObj = new GameObject("PriceLabel");
        labelObj.transform.SetParent(pointObj.transform);
        labelObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        labelObj.transform.localScale = Vector3.one;

        // 添加TextMeshPro组件
        TextMeshPro tmp = labelObj.AddComponent<TextMeshPro>();
        tmp.text = "100.0";
        tmp.fontSize = 0.5f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 10;

        // 设置字体
        tmp.font = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        if (tmp.font == null)
        {
            tmp.font = TMP_Settings.defaultFontAsset;
        }

        // 设置Point脚本的引用
        pointScript.spriteRenderer = spriteRenderer;
        pointScript.priceLabel = tmp;

        // 保存为Prefab
        string prefabPath = "Assets/Source/PointPrefab.prefab";

        // 确保目录存在
        string directory = System.IO.Path.GetDirectoryName(prefabPath);
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
        }

        // 创建Prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(pointObj, prefabPath);

        // 删除场景中的临时对象
        DestroyImmediate(pointObj);

        Debug.Log($"PointPrefab已创建: {prefabPath}");

        // 选中创建的Prefab
        Selection.activeObject = prefab;
    }

    static Texture2D CreateCircleTexture()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        Vector2 center = new Vector2(size / 2, size / 2);
        float radius = size / 2;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                pixels[y * size + x] = distance <= radius ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}