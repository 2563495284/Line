Shader "Custom/TimeAccelerationFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Filter Intensity", Range(0, 1)) = 0.5
        _RadialBlur ("Radial Blur", Range(0, 0.1)) = 0.02
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.02)) = 0.005
        _Distortion ("Radial Distortion", Range(0, 0.1)) = 0.01
        _Brightness ("Brightness", Range(0.5, 2.0)) = 1.2
        _Saturation ("Saturation", Range(0.0, 2.0)) = 1.3
        _TimeSpeed ("Time Speed", Float) = 1.0
        _Center ("Center Point", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        LOD 100
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "TimeAccelerationFilter"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Intensity;
                float _RadialBlur;
                float _ChromaticAberration;
                float _Distortion;
                float _Brightness;
                float _Saturation;
                float _TimeSpeed;
                float4 _Center;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }

            // 径向模糊采样
            half3 RadialBlur(float2 uv, float2 center, float strength, int samples)
            {
                half3 color = half3(0, 0, 0);
                
                float2 direction = (uv - center) * strength;
                
                for (int i = 0; i < samples; i++)
                {
                    float2 sampleUV = uv - direction * (float(i) / float(samples));
                    color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV).rgb;
                }
                
                return color / float(samples);
            }

            // 色彩偏移效果
            half3 ChromaticAberration(float2 uv, float2 center, float strength)
            {
                float2 direction = normalize(uv - center) * strength;
                
                float r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + direction).r;
                float g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).g;
                float b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - direction).b;
                
                return half3(r, g, b);
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

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float2 center = _Center.xy;
                
                // 时间动画
                float time = _Time.y * _TimeSpeed;
                
                // 动态调整效果强度
                float pulseBrightness = 1.0 + 0.1 * sin(time * 3.0);
                float pulseSaturation = 1.0 + 0.2 * sin(time * 2.0);
                float pulseDistortion = 1.0 + 0.3 * sin(time * 4.0);
                
                // 应用径向扭曲
                float2 distortedUV = RadialDistortion(uv, center, _Distortion * _Intensity * pulseDistortion);
                
                // 基础颜色采样
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
                
                // 应用径向模糊
                half3 blurredColor = RadialBlur(distortedUV, center, _RadialBlur * _Intensity, 8);
                
                // 应用色彩偏移
                half3 chromaticColor = ChromaticAberration(distortedUV, center, _ChromaticAberration * _Intensity);
                
                // 混合效果
                half3 finalColor = lerp(baseColor.rgb, blurredColor, _Intensity * 0.6);
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
                
                // 确保颜色范围
                finalColor = saturate(finalColor);
                
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    
    Fallback Off
}
