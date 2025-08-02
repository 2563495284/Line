using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [Header("摄像机设置")]
    public float orthographicSize = 8f;
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    public float nearClipPlane = 0.3f;
    public float farClipPlane = 1000f;

    [Header("摄像机位置")]
    public Vector3 cameraPosition = new Vector3(0, 0, -10);
    public bool autoPosition = true;

    [Header("高级设置")]
    public bool enablePostProcessing = false;
    public bool enableHDR = false;
    public bool enableMSAA = true;

    void Start()
    {
        SetupCamera();
    }

    void SetupCamera()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = gameObject.AddComponent<Camera>();
        }

        // 基础设置
        cam.orthographic = true;
        cam.orthographicSize = orthographicSize;
        cam.backgroundColor = backgroundColor;
        cam.nearClipPlane = nearClipPlane;
        cam.farClipPlane = farClipPlane;

        // 位置设置
        if (autoPosition)
        {
            transform.position = cameraPosition;
        }

        // 高级设置
        cam.allowHDR = enableHDR;
        cam.allowMSAA = enableMSAA;

        // 设置渲染路径
        if (enablePostProcessing)
        {
            cam.renderingPath = RenderingPath.DeferredShading;
        }
        else
        {
            cam.renderingPath = RenderingPath.Forward;
        }

        Debug.Log("摄像机设置完成");
        Debug.Log($"正交大小: {orthographicSize}");
        Debug.Log($"背景色: {backgroundColor}");
        Debug.Log($"位置: {transform.position}");
    }

    // 动态调整摄像机大小
    public void SetOrthographicSize(float size)
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = size;
            orthographicSize = size;
        }
    }

    // 设置背景色
    public void SetBackgroundColor(Color color)
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.backgroundColor = color;
            backgroundColor = color;
        }
    }

    // 适应屏幕
    public void FitToScreen()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            // 根据屏幕比例调整正交大小
            float screenRatio = (float)Screen.width / Screen.height;
            float targetSize = 10f / screenRatio;
            cam.orthographicSize = targetSize;
            orthographicSize = targetSize;
        }
    }

    // 适应内容
    public void FitToContent(float contentWidth, float contentHeight)
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            float screenRatio = (float)Screen.width / Screen.height;
            float contentRatio = contentWidth / contentHeight;

            float targetSize;
            if (screenRatio > contentRatio)
            {
                // 屏幕更宽，以高度为准
                targetSize = contentHeight / 2f;
            }
            else
            {
                // 屏幕更高，以宽度为准
                targetSize = contentWidth / (2f * screenRatio);
            }

            cam.orthographicSize = targetSize;
            orthographicSize = targetSize;
        }
    }

    // 获取当前视野范围
    public Rect GetViewportRect()
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;

            return new Rect(
                transform.position.x - width / 2f,
                transform.position.y - height / 2f,
                width,
                height
            );
        }

        return new Rect();
    }

    // 检查点是否在视野内
    public bool IsInViewport(Vector3 worldPosition)
    {
        Rect viewport = GetViewportRect();
        return viewport.Contains(worldPosition);
    }

    // 将世界坐标转换为屏幕坐标
    public Vector2 WorldToScreenPoint(Vector3 worldPosition)
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            return cam.WorldToScreenPoint(worldPosition);
        }
        return Vector2.zero;
    }

    // 将屏幕坐标转换为世界坐标
    public Vector3 ScreenToWorldPoint(Vector2 screenPosition)
    {
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            return cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -cam.transform.position.z));
        }
        return Vector3.zero;
    }
}