using System.Collections;
using UnityEngine;

/// <summary>
/// Sprite Renderer 时间加速滤镜控制器
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class TimeAccelerationSprite : MonoBehaviour
{
    [Header("滤镜材质")]
    [SerializeField] private Material timeAccelerationMaterial;
    
    [Header("动画参数")]
    [SerializeField] private float transitionDuration = 1.0f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("效果参数")]
    [SerializeField] private float maxIntensity = 0.8f;
    [SerializeField] private float maxRadialBlur = 0.03f;
    [SerializeField] private float maxChromaticAberration = 0.008f;
    [SerializeField] private float maxDistortion = 0.015f;
    [SerializeField] private float maxBrightness = 1.5f;
    [SerializeField] private float maxSaturation = 1.6f;
    [SerializeField] private float timeSpeed = 2.0f;
    
    [Header("调试选项")]
    [SerializeField] private bool showDebugLogs = true;
    [SerializeField] private bool enableManualTesting = false;
    
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Material instanceMaterial;
    private bool isFilterActive = false;
    private Coroutine transitionCoroutine;
    private float currentIntensity = 0f;
    
    // Shader属性ID（性能优化）
    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
    private static readonly int RadialBlurID = Shader.PropertyToID("_RadialBlur");
    private static readonly int ChromaticAberrationID = Shader.PropertyToID("_ChromaticAberration");
    private static readonly int DistortionID = Shader.PropertyToID("_Distortion");
    private static readonly int BrightnessID = Shader.PropertyToID("_Brightness");
    private static readonly int SaturationID = Shader.PropertyToID("_Saturation");
    private static readonly int TimeSpeedID = Shader.PropertyToID("_TimeSpeed");
    private static readonly int CenterID = Shader.PropertyToID("_Center");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeSystem();
    }

    private void OnEnable()
    {
        // 监听天堂制造状态变化
        if (MadeInHeavenSystem.Instance != null)
        {
            MadeInHeavenSystem.OnMadeInHeavenStateChanged += OnMadeInHeavenStateChanged;
        }
    }

    private void OnDisable()
    {
        // 取消监听
        if (MadeInHeavenSystem.Instance != null)
        {
            MadeInHeavenSystem.OnMadeInHeavenStateChanged -= OnMadeInHeavenStateChanged;
        }
        
        // 停止协程
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        // 清理材质实例
        if (instanceMaterial != null)
        {
            DestroyImmediate(instanceMaterial);
        }
    }

    private void InitializeSystem()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("[TimeAccelerationSprite] 找不到SpriteRenderer组件！");
            return;
        }

        // 保存原始材质
        originalMaterial = spriteRenderer.material;
        
        // 验证滤镜材质
        if (timeAccelerationMaterial == null)
        {
            Debug.LogError("[TimeAccelerationSprite] 时间加速滤镜材质未设置！请拖入TimeAccelerationSprite.mat");
            return;
        }

        // 创建材质实例
        instanceMaterial = new Material(timeAccelerationMaterial);
        instanceMaterial.name = "TimeAccelerationSprite (Instance)";
        
        // 初始化材质参数
        UpdateMaterialProperties();

        if (showDebugLogs)
        {
            Debug.Log("[TimeAccelerationSprite] Sprite滤镜系统初始化完成");
        }
    }

    /// <summary>
    /// 响应天堂制造状态变化
    /// </summary>
    private void OnMadeInHeavenStateChanged(bool isActive)
    {
        if (showDebugLogs)
        {
            Debug.Log($"[TimeAccelerationSprite] 天堂制造状态变化: {(isActive ? "激活" : "停用")}");
        }

        if (isActive)
        {
            StartTimeAccelerationFilter();
        }
        else
        {
            StopTimeAccelerationFilter();
        }
    }

    /// <summary>
    /// 启动时间加速滤镜
    /// </summary>
    public void StartTimeAccelerationFilter()
    {
        if (isFilterActive || instanceMaterial == null)
            return;

        isFilterActive = true;
        
        // 应用滤镜材质
        spriteRenderer.material = instanceMaterial;

        // 停止之前的过渡
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        // 开始淡入过渡
        transitionCoroutine = StartCoroutine(TransitionFilter(0f, 1f));

        if (showDebugLogs)
        {
            Debug.Log("[TimeAccelerationSprite] Sprite时间加速滤镜已启动");
        }
    }

    /// <summary>
    /// 停止时间加速滤镜
    /// </summary>
    public void StopTimeAccelerationFilter()
    {
        if (!isFilterActive)
            return;

        // 停止之前的过渡
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        // 开始淡出过渡
        transitionCoroutine = StartCoroutine(TransitionFilter(currentIntensity, 0f));

        if (showDebugLogs)
        {
            Debug.Log("[TimeAccelerationSprite] Sprite时间加速滤镜已停止");
        }
    }

    /// <summary>
    /// 滤镜过渡协程
    /// </summary>
    private IEnumerator TransitionFilter(float fromIntensity, float toIntensity)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(progress);

            currentIntensity = Mathf.Lerp(fromIntensity, toIntensity, curveValue);
            UpdateMaterialProperties();

            yield return null;
        }

        currentIntensity = toIntensity;
        UpdateMaterialProperties();

        // 如果淡出完成，恢复原始材质
        if (Mathf.Approximately(toIntensity, 0f))
        {
            isFilterActive = false;
            spriteRenderer.material = originalMaterial;
        }

        transitionCoroutine = null;
    }

    /// <summary>
    /// 更新材质属性
    /// </summary>
    private void UpdateMaterialProperties()
    {
        if (instanceMaterial == null)
            return;

        instanceMaterial.SetFloat(IntensityID, currentIntensity);
        instanceMaterial.SetFloat(RadialBlurID, maxRadialBlur * currentIntensity);
        instanceMaterial.SetFloat(ChromaticAberrationID, maxChromaticAberration * currentIntensity);
        instanceMaterial.SetFloat(DistortionID, maxDistortion * currentIntensity);
        instanceMaterial.SetFloat(BrightnessID, Mathf.Lerp(1f, maxBrightness, currentIntensity));
        instanceMaterial.SetFloat(SaturationID, Mathf.Lerp(1f, maxSaturation, currentIntensity));
        instanceMaterial.SetFloat(TimeSpeedID, timeSpeed);
        instanceMaterial.SetVector(CenterID, new Vector4(0.5f, 0.5f, 0f, 0f));

        if (showDebugLogs && currentIntensity > 0)
        {
            Debug.Log($"[TimeAccelerationSprite] 材质参数已更新 - 强度: {currentIntensity:F2}");
        }
    }

    /// <summary>
    /// 设置滤镜强度（手动控制）
    /// </summary>
    public void SetFilterIntensity(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        currentIntensity = intensity;
        isFilterActive = intensity > 0f;
        
        // 根据强度决定使用哪个材质
        spriteRenderer.material = isFilterActive ? instanceMaterial : originalMaterial;
        
        UpdateMaterialProperties();

        if (showDebugLogs)
        {
            Debug.Log($"[TimeAccelerationSprite] 滤镜强度设置为: {intensity:F2}");
        }
    }

    // 调试和测试功能
    private void Update()
    {
        if (enableManualTesting && Application.isEditor)
        {
            HandleDebugInput();
        }
    }

    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartTimeAccelerationFilter();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            StopTimeAccelerationFilter();
        }

        if (Input.GetKey(KeyCode.F3))
        {
            float intensity = Mathf.PingPong(Time.time, 1f);
            SetFilterIntensity(intensity);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("[TimeAccelerationSprite] F4键 - 强制设置高强度滤镜");
            SetFilterIntensity(1.0f);
        }
    }

    /// <summary>
    /// 测试方法：直接设置高强度效果
    /// </summary>
    [ContextMenu("测试最大强度滤镜")]
    public void TestMaxIntensityFilter()
    {
        Debug.Log("[TimeAccelerationSprite] 测试最大强度滤镜");
        SetFilterIntensity(1.0f);
    }

    #region 公共属性

    /// <summary>
    /// 滤镜是否激活
    /// </summary>
    public bool IsFilterActive => isFilterActive;

    /// <summary>
    /// 当前滤镜强度
    /// </summary>
    public float CurrentIntensity => currentIntensity;

    /// <summary>
    /// 获取状态信息
    /// </summary>
    public string GetStatusInfo()
    {
        return $"Sprite时间加速滤镜: {(isFilterActive ? "激活" : "停用")} (强度: {currentIntensity:F2})";
    }

    #endregion
}
