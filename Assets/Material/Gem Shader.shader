Shader "Custom/SpriteHueToEmerald"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _TargetHue("Target Hue (Emerald ≈ 0.38)", Range(0, 1)) = 0.38
    }
    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "PreviewType" = "Sprite"
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
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _TargetHue;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // RGB to HSV
            float3 RGBToHSV(float3 c)
            {
                float4 K = float4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
                float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
                float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);
                float d = q.x - min(q.w, q.y);
                float e = 1e-10;
                return float3(abs((q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            // HSV to RGB
            float3 HSVToRGB(float3 hsv)
            {
                float3 rgb = clamp(abs(frac(hsv.x + float3(0, 2.0/3.0, 1.0/3.0)) * 6.0 - 3.0) - 1.0, 0.0, 1.0);
                return hsv.z * lerp(float3(1.0, 1.0, 1.0), rgb, hsv.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv);
                float3 hsv = RGBToHSV(tex.rgb);
                hsv.x = _TargetHue; // Replace hue with emerald
                float3 emeraldColor = HSVToRGB(hsv);
                return float4(emeraldColor, tex.a);
            }
            ENDCG
        }
    }
}
