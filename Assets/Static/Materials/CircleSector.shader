Shader "Custom/CenteredEllipseSector"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _EllipseColor ("Ellipse Color", Color) = (1,1,1,1)
        _SectorColor ("Sector Color", Color) = (1,1,0.5,0.7)
        _StartAngle ("Start Angle (degrees)", Range(0, 360)) = 0
        _EndAngle ("End Angle (degrees)", Range(0, 360)) = 90
        _LineWidth ("Line Width", Range(0.001, 0.1)) = 0.01
        _ShowEllipse ("Show Ellipse", Int) = 1
        _ShowSector ("Show Sector", Int) = 1
        _UseSpriteAlpha ("Use Sprite Alpha", Int) = 1
        
        // 内置属性保持不变
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

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
                float2 localPos : TEXCOORD1; // 局部坐标，用于计算与中心的相对位置
                float2 spriteSize : TEXCOORD2; // Sprite的宽高信息
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _EllipseColor;
            fixed4 _SectorColor;
            float _StartAngle;
            float _EndAngle;
            float _LineWidth;
            int _ShowEllipse;
            int _ShowSector;
            int _UseSpriteAlpha;
            float4 _Flip;
            float4 _MainTex_ST; // 用于获取纹理缩放信息

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                // 获取Sprite的尺寸信息（通过纹理缩放计算）
                OUT.spriteSize = float2(_MainTex_ST.x, _MainTex_ST.y);
                
                // 计算局部坐标（0到1范围）
                OUT.localPos = IN.texcoord;
                
                IN.vertex.xy *= _Flip.xy;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D (_AlphaTex, uv);
                color.a = alpha.r;
                #endif //ETC1_EXTERNAL_ALPHA

                return color;
            }

            // 角度转换：度转弧度
            float DegToRad(float degrees)
            {
                return degrees * UNITY_PI / 180.0;
            }

            // 计算点是否在扇形内
            bool IsInSector(float2 pos, float startAngle, float endAngle)
            {
                // 计算点到中心的角度（弧度）
                float angle = atan2(pos.y, pos.x);
                
                // 标准化到0-2π范围
                if (angle < 0) angle += 2 * UNITY_PI;
                
                // 处理跨越0度的情况
                if (startAngle <= endAngle)
                {
                    return angle >= startAngle && angle <= endAngle;
                }
                else
                {
                    return angle >= startAngle || angle <= endAngle;
                }
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 采样精灵纹理
                fixed4 spriteColor = SampleSpriteTexture(IN.texcoord) * IN.color;
                
                // 计算相对于Sprite中心的位置（原点在中心，范围：-0.5到0.5）
                float2 centerOffset = IN.localPos - 0.5;
                
                // 考虑Sprite的宽高比，确保椭圆正确内接于矩形
                float aspectRatio = IN.spriteSize.x / IN.spriteSize.y;
                
                // 标准化坐标（消除宽高比影响，使椭圆完美内接）
                float2 normalizedPos;
                normalizedPos.x = centerOffset.x * 2 * aspectRatio; // X方向根据宽高比调整
                normalizedPos.y = centerOffset.y * 2; // Y方向保持不变
                
                // 椭圆方程：(x/a)^2 + (y/b)^2 = 1
                // 这里a和b设为1，因为我们已经通过normalizedPos处理了比例
                float ellipseValue = (normalizedPos.x * normalizedPos.x) + (normalizedPos.y * normalizedPos.y);
                
                // 计算是否在椭圆线条上（考虑线宽）
                float lineInner = 1.0 - _LineWidth * 50.0 * min(IN.spriteSize.x, IN.spriteSize.y);
                float lineOuter = 1.0 + _LineWidth * 50.0 * min(IN.spriteSize.x, IN.spriteSize.y);
                float inEllipseLine = step(ellipseValue, lineOuter) - step(ellipseValue, lineInner);
                
                // 转换角度为弧度
                float startRad = DegToRad(_StartAngle);
                float endRad = DegToRad(_EndAngle);
                
                // 计算是否在扇形内
                bool inSector = IsInSector(normalizedPos, startRad, endRad) && ellipseValue <= 1.0;
                
                // 计算扇形边界（两条半径）
                float2 startDir = float2(cos(startRad), sin(startRad));
                float2 endDir = float2(cos(endRad), sin(endRad));
                
                // 计算到两条半径的距离（用于绘制边界线）
                float crossStart = abs(dot(normalizedPos, float2(-startDir.y, startDir.x))) / length(normalizedPos);
                float crossEnd = abs(dot(normalizedPos, float2(-endDir.y, endDir.x))) / length(normalizedPos);
                float inRadiusLine = (step(crossStart, _LineWidth * 100.0) + step(crossEnd, _LineWidth * 100.0)) * step(0.001, length(normalizedPos));
                
                // 混合颜色
                fixed4 finalColor = fixed4(0, 0, 0, 0);
                
                // 绘制扇形内部
                if (_ShowSector && inSector)
                {
                    finalColor = lerp(finalColor, _SectorColor, _SectorColor.a);
                }
                
                // 绘制椭圆边界
                if (_ShowEllipse && inEllipseLine > 0)
                {
                    finalColor = lerp(finalColor, _EllipseColor, _EllipseColor.a);
                }
                
                // 绘制扇形半径边界
                if (_ShowSector && inRadiusLine > 0)
                {
                    finalColor = lerp(finalColor, _EllipseColor, _EllipseColor.a);
                }
                
                // 如果启用精灵Alpha，将扇形和椭圆与精灵的Alpha混合
                if (_UseSpriteAlpha)
                {
                    finalColor.a *= spriteColor.a;
                }
                
                return finalColor;
            }
        ENDCG
        }
    }
}
    