Shader "Sprites/TimeAccelerationSprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Time Acceleration Effects)]
        _Intensity ("Filter Intensity", Range(0, 1)) = 0.5
        _RadialBlur ("Radial Blur", Range(0, 0.1)) = 0.02
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.02)) = 0.005
        _Distortion ("Radial Distortion", Range(0, 0.1)) = 0.01
        _Brightness ("Brightness", Range(0.5, 2.0)) = 1.2
        _Saturation ("Saturation", Range(0.0, 2.0)) = 1.3
        _TimeSpeed ("Time Speed", Float) = 1.0
        _Center ("Center Point", Vector) = (0.5, 0.5, 0, 0)

        [Header(Sprite Settings)]
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
            #include "UnitySprites.cginc"

            // Time Acceleration Properties
            float _Intensity;
            float _RadialBlur;
            float _ChromaticAberration;
            float _Distortion;
            float _Brightness;
            float _Saturation;
            float _TimeSpeed;
            float4 _Center;

            // 径向模糊采样
            fixed3 RadialBlur(float2 uv, float2 center, float strength, int samples)
            {
                fixed3 color = fixed3(0, 0, 0);
                
                float2 direction = (uv - center) * strength;
                
                for (int i = 0; i < samples; i++)
                {
                    float2 sampleUV = uv - direction * (float(i) / float(samples));
                    color += tex2D(_MainTex, sampleUV).rgb;
                }
                
                return color / float(samples);
            }

            // 色彩偏移效果
            fixed3 ChromaticAberration(float2 uv, float2 center, float strength)
            {
                float2 direction = normalize(uv - center) * strength;
                
                float r = tex2D(_MainTex, uv + direction).r;
                float g = tex2D(_MainTex, uv).g;
                float b = tex2D(_MainTex, uv - direction).b;
                
                return fixed3(r, g, b);
            }

            // 径向扭曲
            float2 RadialDistortion(float2 uv, float2 center, float strength)
            {
                float2 direction = uv - center;
                float distance = length(direction);
                
                float distortion = 1.0 + strength * distance * distance;
                return center + direction * distortion;
            }

            // RGB转HSV
            float3 RGBtoHSV(float3 rgb)
            {
                float4 k = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(rgb.bg, k.wz), float4(rgb.gb, k.xy), step(rgb.b, rgb.g));
                float4 q = lerp(float4(p.xyw, rgb.r), float4(rgb.r, p.yzx), step(p.x, rgb.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            // HSV转RGB
            float3 HSVtoRGB(float3 hsv)
            {
                float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(hsv.xxx + k.xyz) * 6.0 - k.www);
                return hsv.z * lerp(k.xxx, clamp(p - k.xxx, 0.0, 1.0), hsv.y);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float2 center = _Center.xy;
                
                // 时间动画
                float time = _Time.y * _TimeSpeed;
                
                // 动态调整效果强度
                float pulseBrightness = 1.0 + 0.1 * sin(time * 3.0);
                float pulseSaturation = 1.0 + 0.2 * sin(time * 2.0);
                float pulseDistortion = 1.0 + 0.3 * sin(time * 4.0);
                
                // 应用径向扭曲
                float2 distortedUV = RadialDistortion(uv, center, _Distortion * _Intensity * pulseDistortion);
                
                // 基础Sprite颜色采样
                fixed4 c = SampleSpriteTexture(distortedUV) * IN.color;
                
                // 应用时间加速效果
                if (_Intensity > 0.01)
                {
                    // 应用径向模糊
                    fixed3 blurredColor = RadialBlur(distortedUV, center, _RadialBlur * _Intensity, 8);
                    
                    // 应用色彩偏移
                    fixed3 chromaticColor = ChromaticAberration(distortedUV, center, _ChromaticAberration * _Intensity);
                    
                    // 混合效果
                    fixed3 finalColor = lerp(c.rgb, blurredColor, _Intensity * 0.6);
                    finalColor = lerp(finalColor, chromaticColor, _Intensity * 0.4);
                    
                    // 调整亮度和饱和度
                    finalColor *= _Brightness * pulseBrightness;
                    
                    // 转换到HSV空间调整饱和度
                    float3 hsv = RGBtoHSV(finalColor);
                    hsv.y *= _Saturation * pulseSaturation * _Intensity;
                    finalColor = HSVtoRGB(hsv);
                    
                    // 添加边缘暗化效果
                    float2 centerDistance = abs(uv - center);
                    float vignette = 1.0 - smoothstep(0.3, 0.8, length(centerDistance));
                    vignette = lerp(1.0, vignette, _Intensity * 0.3);
                    finalColor *= vignette;
                    
                    // 添加轻微的色调偏移
                    float hueShift = sin(time * 1.5) * 0.1 * _Intensity;
                    hsv = RGBtoHSV(finalColor);
                    hsv.x += hueShift;
                    finalColor = HSVtoRGB(hsv);
                    
                    c.rgb = saturate(finalColor);
                }

                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
