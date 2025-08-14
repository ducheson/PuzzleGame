Shader "Custom/ShinySprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ShineColor ("Shine Color", Color) = (1,1,1,0.8)
        _ShineWidth ("Shine Width", Range(0.05, 0.5)) = 0.2
        _ShineSpeed ("Shine Speed", Range(0.1, 5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
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
            float4 _ShineColor;
            float _ShineWidth;
            float _ShineSpeed;
            float4 _MainTex_ST;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate shine position (moves across UV.x over time)
                float shinePos = frac(_Time.y * _ShineSpeed);

                // Calculate distance from current pixel's UV.x to shinePos
                float dist = abs(i.uv.x - shinePos);

                // Create a smooth highlight based on distance and width
                float shine = smoothstep(_ShineWidth, 0.0, dist);

                // Add shine color weighted by the shine intensity
                fixed3 finalColor = col.rgb + _ShineColor.rgb * shine * col.a;

                return fixed4(finalColor, col.a);
            }
            ENDCG
        }
    }
}
