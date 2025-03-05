Shader "Custom/MinimapFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogTex ("Fog Texture", 2D) = "black" {}
        _Zoom ("Zoom", Float) = 1.0
        _Offset ("Offset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _FogTex;
            float _Zoom;
            float2 _Offset;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
    
                float2 center = float2(0.5, 0.5);
                o.uv = center + ((v.uv - center) / _Zoom) + (_Offset / _Zoom);  //Ajuste de Offset relativo al Zoom

                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                float2 clampedUV = clamp(i.uv, 0.0, 1.0); // Evita repetir la textura
                fixed4 col = tex2D(_MainTex, clampedUV);
                fixed4 fog = tex2D(_FogTex, clampedUV);
                return col * step(0.01, 1 - fog.a); // Ajuste para evitar interpolación

            }

            ENDCG
        }
    }
}
