Shader "Custom/TimeAccelerationUI"
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

        [Header(UI Settings)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_TexelSize;

            // Time Acceleration Properties
            float _Intensity;
            float _RadialBlur;
            float _ChromaticAberration;
            float _Distortion;
            float _Brightness;
            float _Saturation;
            float _TimeSpeed;
            float4 _Center;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = v.texcoord;

                OUT.color = v.color * _Color;
                return OUT;
            }

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
                
                // 基础颜色采样
                half4 color = (tex2D(_MainTex, distortedUV) + _TextureSampleAdd) * IN.color;
                
                // 应用时间加速效果
                if (_Intensity > 0.01)
                {
                    // 应用径向模糊
                    fixed3 blurredColor = RadialBlur(distortedUV, center, _RadialBlur * _Intensity, 8);
                    
                    // 应用色彩偏移
                    fixed3 chromaticColor = ChromaticAberration(distortedUV, center, _ChromaticAberration * _Intensity);
                    
                    // 混合效果
                    fixed3 finalColor = lerp(color.rgb, blurredColor, _Intensity * 0.6);
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
                    
                    color.rgb = saturate(finalColor);
                }

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
