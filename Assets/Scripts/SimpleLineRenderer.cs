using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleLineRenderer : MonoBehaviour
{
    [Header("线条设置")]
    public Color lineColor = Color.white;
    public float lineWidth = 2f;
    public Material lineMaterial;

    [Header("渲染设置")]
    public int textureSize = 512;
    public bool useAntiAliasing = true;

    private RawImage rawImage;
    private Texture2D lineTexture;
    private List<Vector2> points = new List<Vector2>();

    void Start()
    {
        InitializeRenderer();
    }

    void InitializeRenderer()
    {
        // 获取或创建RawImage组件
        rawImage = GetComponent<RawImage>();
        if (rawImage == null)
        {
            rawImage = gameObject.AddComponent<RawImage>();
        }

        // 创建线条材质
        if (lineMaterial == null)
        {
            lineMaterial = new Material(Shader.Find("UI/Default"));
        }

        // 创建纹理
        CreateLineTexture();

        rawImage.material = lineMaterial;
        rawImage.texture = lineTexture;
    }

    void CreateLineTexture()
    {
        lineTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        lineTexture.filterMode = useAntiAliasing ? FilterMode.Bilinear : FilterMode.Point;
        lineTexture.wrapMode = TextureWrapMode.Clamp;

        // 清空纹理
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        lineTexture.SetPixels(pixels);
        lineTexture.Apply();
    }

    public void SetPoints(List<Vector2> newPoints)
    {
        points = new List<Vector2>(newPoints);
        RenderLines();
    }

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
        RenderLines();
    }

    public void ClearPoints()
    {
        points.Clear();
        ClearTexture();
    }

    void RenderLines()
    {
        if (points.Count < 2)
            return;

        ClearTexture();

        // 绘制线条
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(points[i], points[i + 1]);
        }

        lineTexture.Apply();
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        // 将世界坐标转换为纹理坐标
        Vector2 startTex = WorldToTexture(start);
        Vector2 endTex = WorldToTexture(end);

        // 使用Bresenham算法绘制线条
        int x0 = Mathf.RoundToInt(startTex.x);
        int y0 = Mathf.RoundToInt(startTex.y);
        int x1 = Mathf.RoundToInt(endTex.x);
        int y1 = Mathf.RoundToInt(endTex.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // 绘制当前像素
            if (x0 >= 0 && x0 < textureSize && y0 >= 0 && y0 < textureSize)
            {
                lineTexture.SetPixel(x0, y0, lineColor);
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    Vector2 WorldToTexture(Vector2 worldPos)
    {
        // 将世界坐标转换为纹理坐标
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null) return Vector2.zero;

        // 获取RectTransform的边界
        Vector2 size = rect.sizeDelta;
        Vector2 min = -size / 2f;
        Vector2 max = size / 2f;

        // 归一化坐标
        float x = Mathf.InverseLerp(min.x, max.x, worldPos.x);
        float y = Mathf.InverseLerp(min.y, max.y, worldPos.y);

        // 转换为纹理坐标
        return new Vector2(x * textureSize, y * textureSize);
    }

    void ClearTexture()
    {
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        lineTexture.SetPixels(pixels);
    }

    public void SetLineColor(Color color)
    {
        lineColor = color;
        if (points.Count >= 2)
        {
            RenderLines();
        }
    }

    public void SetLineWidth(float width)
    {
        lineWidth = width;
        if (points.Count >= 2)
        {
            RenderLines();
        }
    }

    void OnDestroy()
    {
        if (lineTexture != null)
        {
            DestroyImmediate(lineTexture);
        }
        if (lineMaterial != null)
        {
            DestroyImmediate(lineMaterial);
        }
    }
}