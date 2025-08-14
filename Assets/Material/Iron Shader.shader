Shader "Custom/IronCoinSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Darkness ("Darkness", Range(0,1)) = 0.15
        _TintColor ("Tint Color", Color) = (0.6, 0.65, 0.7, 1) // Slightly lighter bluish gray
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Sprite"
            "DisableBatching"="True"
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
            float4 _MainTex_ST;
            float _Darkness;
            fixed4 _TintColor;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;

                // Convert to grayscale (luminance)
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Add brightness offset after darkness multiplier
                gray = saturate(gray * _Darkness + 0.05);

                // Apply tint color multiplied by gray level
                float3 tinted = gray * _TintColor.rgb;

                return fixed4(tinted, col.a);
            }
            ENDCG
        }
    }
}