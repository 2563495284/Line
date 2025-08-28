using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// URP时间加速滤镜渲染特性
/// </summary>
public class TimeAccelerationRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Header("滤镜材质")]
        public Material material;

        [Header("渲染设置")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        [Header("效果参数")]
        public bool isEnabled = true;
    }

    public Settings settings = new Settings();
    private TimeAccelerationRenderPass renderPass;

    public override void Create()
    {
        renderPass = new TimeAccelerationRenderPass(settings);
    }

    /// <summary>
    /// 检查时间加速是否激活
    /// </summary>
    private bool IsTimeAccelerationActive()
    {
        // 通过反射安全地访问静态属性
        try
        {
            var filterSystemType = System.Type.GetType("TimeAccelerationFilterSystem");
            if (filterSystemType != null)
            {
                var globalActiveProperty = filterSystemType.GetProperty("GlobalFilterActive");
                if (globalActiveProperty != null)
                {
                    return (bool)globalActiveProperty.GetValue(null);
                }
            }
        }
        catch
        {
            // 如果反射失败，返回false
        }
        return false;
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData
    )
    {
        Debug.Log("[TimeAccelerationRenderFeature] AddRenderPasses被调用");

        if (settings.material == null || !settings.isEnabled)
        {
            Debug.LogWarning($"[TimeAccelerationRenderFeature] 跳过渲染 - 材质为空: {settings.material == null}, 未启用: {!settings.isEnabled}");
            return;
        }

        Debug.Log($"[TimeAccelerationRenderFeature] 材质已设置: {settings.material.name}");

        // 检查是否需要渲染滤镜效果
        bool isActive = IsTimeAccelerationActive();
        Debug.Log($"[TimeAccelerationRenderFeature] 滤镜系统激活状态: {isActive}");

        if (!isActive)
        {
            Debug.Log("[TimeAccelerationRenderFeature] 滤镜未激活，跳过渲染");
            return;
        }

        Debug.Log("[TimeAccelerationRenderFeature] 添加渲染通道到队列");
        // 不在这里获取相机颜色目标，而是在Execute中获取
        renderer.EnqueuePass(renderPass);
    }

    /// <summary>
    /// 时间加速滤镜渲染通道
    /// </summary>
    public class TimeAccelerationRenderPass : ScriptableRenderPass
    {
        private Settings settings;
        private int tempTextureId;

        private const string ProfilerTag = "TimeAccelerationFilter";

        public TimeAccelerationRenderPass(Settings settings)
        {
            this.settings = settings;
            renderPassEvent = settings.renderPassEvent;
            tempTextureId = Shader.PropertyToID("_TimeAccelerationTempTexture");
        }

        /// <summary>
        /// 在渲染通道中检查时间加速是否激活
        /// </summary>
        private bool IsTimeAccelerationActiveInPass()
        {
            // 通过反射安全地访问静态属性
            try
            {
                var filterSystemType = System.Type.GetType("TimeAccelerationFilterSystem");
                if (filterSystemType != null)
                {
                    var globalActiveProperty = filterSystemType.GetProperty("GlobalFilterActive");
                    if (globalActiveProperty != null)
                    {
                        return (bool)globalActiveProperty.GetValue(null);
                    }
                }
            }
            catch
            {
                // 如果反射失败，返回false
            }
            return false;
        }

        public override void Execute(
            ScriptableRenderContext context,
            ref RenderingData renderingData
        )
        {
            // 调试：始终输出执行信息
            Debug.Log("[TimeAccelerationRenderPass] Execute方法被调用");

            if (settings.material == null)
            {
                Debug.LogError("[TimeAccelerationRenderPass] 材质为空！请检查RenderFeature设置");
                return;
            }

            // 调试：检查材质
            Debug.Log($"[TimeAccelerationRenderPass] 使用材质: {settings.material.name}");

            // 检查滤镜系统是否激活
            bool isActive = IsTimeAccelerationActiveInPass();
            Debug.Log($"[TimeAccelerationRenderPass] 滤镜系统激活状态: {isActive}");

            if (!isActive)
                return;

            Debug.Log("[TimeAccelerationRenderPass] 开始应用滤镜效果");

            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);

            // 在Execute方法中安全地获取相机颜色目标
            var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
            var cameraDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            cameraDescriptor.depthBufferBits = 0;

            Debug.Log($"[TimeAccelerationRenderPass] 相机分辨率: {cameraDescriptor.width}x{cameraDescriptor.height}");

            // 获取临时渲染纹理
            cmd.GetTemporaryRT(tempTextureId, cameraDescriptor);

            // 应用滤镜效果：从相机颜色目标到临时纹理，再回到相机颜色目标
            cmd.Blit(cameraColorTarget, tempTextureId, settings.material);
            cmd.Blit(tempTextureId, cameraColorTarget);

            Debug.Log("[TimeAccelerationRenderPass] 滤镜效果已应用");

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTextureId);
        }
    }
}
