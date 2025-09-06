using System.Collections;
using UnityEngine;

/// <summary>
/// URP兼容的时间加速滤镜系统
/// </summary>
public class TimeAccelerationFilterSystem : Singleton<TimeAccelerationFilterSystem>
{
    [Header("滤镜设置")]
    [SerializeField]
    private Material timeAccelerationMaterial;

    [Header("动画参数")]
    [SerializeField]
    private float transitionDuration = 1.0f;

    [SerializeField]
    private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("效果参数")]
    [SerializeField]
    private float maxIntensity = 0.8f;

    [SerializeField]
    private float maxRadialBlur = 0.03f;

    [SerializeField]
    private float maxChromaticAberration = 0.008f;

    [SerializeField]
    private float maxDistortion = 0.015f;

    [SerializeField]
    private float maxBrightness = 1.5f;

    [SerializeField]
    private float maxSaturation = 1.6f;

    [SerializeField]
    private float timeSpeed = 2.0f;

    [Header("调试选项")]
    [SerializeField]
    private bool showDebugLogs = true;

    [SerializeField]
    private bool enableManualTesting = false;

    private bool isFilterActive = false;
    private Coroutine transitionCoroutine;
    private float currentIntensity = 0f;

    // 静态属性供RenderFeature使用
    public static bool GlobalFilterActive { get; private set; } = false;

    // Shader属性ID（性能优化）
    private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
    private static readonly int RadialBlurID = Shader.PropertyToID("_RadialBlur");
    private static readonly int ChromaticAberrationID = Shader.PropertyToID("_ChromaticAberration");
    private static readonly int DistortionID = Shader.PropertyToID("_Distortion");
    private static readonly int BrightnessID = Shader.PropertyToID("_Brightness");
    private static readonly int SaturationID = Shader.PropertyToID("_Saturation");
    private static readonly int TimeSpeedID = Shader.PropertyToID("_TimeSpeed");
    private static readonly int CenterID = Shader.PropertyToID("_Center");

    protected override void Awake()
    {
        base.Awake();
        InitializeSystem();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {


        // 停止协程
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }

    private void InitializeSystem()
    {
        // 验证材质
        if (timeAccelerationMaterial == null)
        {
            Debug.LogError(
                "[TimeAccelerationFilterSystem] 时间加速滤镜材质未设置！请在Inspector中拖入TimeAccelerationFilter.mat"
            );
            return;
        }

        Debug.Log($"[TimeAccelerationFilterSystem] 已设置材质: {timeAccelerationMaterial.name}");

        // 初始化材质参数
        UpdateMaterialProperties();

        if (showDebugLogs)
        {
            Debug.Log("[TimeAccelerationFilterSystem] URP系统初始化完成");
        }
    }

    /// <summary>
    /// 响应天堂制造状态变化
    /// </summary>
    private void OnMadeInHeavenStateChanged(bool isActive)
    {
        if (showDebugLogs)
        {
            Debug.Log(
                $"[TimeAccelerationFilterSystem] 天堂制造状态变化: {(isActive ? "激活" : "停用")}"
            );
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
        if (isFilterActive)
            return;

        isFilterActive = true;
        GlobalFilterActive = true;

        // 停止之前的过渡
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        // 开始淡入过渡
        transitionCoroutine = StartCoroutine(TransitionFilter(0f, 1f));

        if (showDebugLogs)
        {
            Debug.Log("[TimeAccelerationFilterSystem] URP时间加速滤镜已启动");
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
            Debug.Log("[TimeAccelerationFilterSystem] URP时间加速滤镜已停止");
        }
    }

    /// <summary>
    /// 滤镜过渡协程
    /// </summary>
    private System.Collections.IEnumerator TransitionFilter(float fromIntensity, float toIntensity)
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

        // 如果淡出完成，标记滤镜为非激活状态
        if (Mathf.Approximately(toIntensity, 0f))
        {
            isFilterActive = false;
            GlobalFilterActive = false;
        }

        transitionCoroutine = null;
    }

    /// <summary>
    /// 更新材质属性
    /// </summary>
    private void UpdateMaterialProperties()
    {
        if (timeAccelerationMaterial == null)
        {
            Debug.LogWarning("[TimeAccelerationFilterSystem] 更新材质属性失败：材质为空");
            return;
        }

        Debug.Log($"[TimeAccelerationFilterSystem] 更新材质属性 - Intensity: {currentIntensity}");

        timeAccelerationMaterial.SetFloat(IntensityID, currentIntensity);
        timeAccelerationMaterial.SetFloat(RadialBlurID, maxRadialBlur * currentIntensity);
        timeAccelerationMaterial.SetFloat(
            ChromaticAberrationID,
            maxChromaticAberration * currentIntensity
        );
        timeAccelerationMaterial.SetFloat(DistortionID, maxDistortion * currentIntensity);
        timeAccelerationMaterial.SetFloat(
            BrightnessID,
            Mathf.Lerp(1f, maxBrightness, currentIntensity)
        );
        timeAccelerationMaterial.SetFloat(
            SaturationID,
            Mathf.Lerp(1f, maxSaturation, currentIntensity)
        );
        timeAccelerationMaterial.SetFloat(TimeSpeedID, timeSpeed);
        timeAccelerationMaterial.SetVector(CenterID, new Vector4(0.5f, 0.5f, 0f, 0f));

        Debug.Log($"[TimeAccelerationFilterSystem] 材质参数已更新 - RadialBlur: {maxRadialBlur * currentIntensity}, ChromaticAberration: {maxChromaticAberration * currentIntensity}");
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
        GlobalFilterActive = isFilterActive;
        UpdateMaterialProperties();

        if (showDebugLogs)
        {
            Debug.Log($"[TimeAccelerationFilterSystem] URP滤镜强度设置为: {intensity:F2}");
        }
    }

    /// <summary>
    /// 平滑设置滤镜强度
    /// </summary>
    public void SetFilterIntensitySmooth(float intensity, float duration = -1)
    {
        intensity = Mathf.Clamp01(intensity);

        if (duration < 0)
            duration = transitionDuration;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionToIntensity(intensity, duration));

        if (showDebugLogs)
        {
            Debug.Log(
                $"[TimeAccelerationFilterSystem] 平滑设置滤镜强度为: {intensity:F2}，持续时间: {duration:F1}s"
            );
        }
    }

    /// <summary>
    /// 过渡到指定强度的协程
    /// </summary>
    private System.Collections.IEnumerator TransitionToIntensity(float targetIntensity, float duration)
    {
        float startIntensity = currentIntensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float curveValue = transitionCurve.Evaluate(progress);

            currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, curveValue);
            isFilterActive = currentIntensity > 0f;
            GlobalFilterActive = isFilterActive;
            UpdateMaterialProperties();

            yield return null;
        }

        currentIntensity = targetIntensity;
        isFilterActive = currentIntensity > 0f;
        GlobalFilterActive = isFilterActive;
        UpdateMaterialProperties();

        transitionCoroutine = null;
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

        // 添加强制高强度测试
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Debug.Log("[TimeAccelerationFilterSystem] F4键 - 强制设置高强度滤镜");
            SetFilterIntensity(1.0f);
        }
    }

    /// <summary>
    /// 测试方法：直接设置高强度效果
    /// </summary>
    [ContextMenu("测试最大强度滤镜")]
    public void TestMaxIntensityFilter()
    {
        Debug.Log("[TimeAccelerationFilterSystem] 测试最大强度滤镜");
        isFilterActive = true;
        GlobalFilterActive = true;
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
    /// 设置材质引用（用于RenderFeature）
    /// </summary>
    public void SetMaterial(Material material)
    {
        timeAccelerationMaterial = material;
        if (timeAccelerationMaterial != null)
        {
            UpdateMaterialProperties();
        }
    }

    /// <summary>
    /// 获取状态信息
    /// </summary>
    public string GetStatusInfo()
    {
        return $"URP时间加速滤镜: {(isFilterActive ? "激活" : "停用")} (强度: {currentIntensity:F2})";
    }

    /// <summary>
    /// 设置最大强度
    /// </summary>
    public void SetMaxIntensity(float intensity) => maxIntensity = Mathf.Clamp01(intensity);

    /// <summary>
    /// 设置时间速度
    /// </summary>
    public void SetTimeSpeed(float speed) => timeSpeed = Mathf.Max(0.1f, speed);

    #endregion
}
