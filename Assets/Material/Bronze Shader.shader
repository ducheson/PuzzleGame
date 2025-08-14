Shader "Custom/BronzeTintSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BronzeColor ("Bronze Tint Color", Color) = (0.804, 0.498, 0.196, 1) // Bronze-ish color
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
            fixed4 _BronzeColor;

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
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Multiply original color by bronze tint (preserves alpha)
                fixed3 bronze = _BronzeColor.rgb;

                // You can also try to mix bronze with grayscale or do other fancy tinting
                fixed3 tinted = col.rgb * bronze;

                return fixed4(tinted, col.a);
            }
            ENDCG
        }
    }
}
