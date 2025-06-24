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

                // Aplica zoom correctamente sin cambiar el centro
                float2 zoomedUV = (v.uv - center) / _Zoom + center;

                // Aplica offset de manera proporcional al zoom
                float2 offsetUV = zoomedUV + (_Offset / (_Zoom * 2.0));

                // Restringe los valores de UV pero permite moverse dentro de los límites
                o.uv = clamp(offsetUV, 0.0, 1.0);

                return o;
            }




            /*v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
    
                float2 center = float2(0.5, 0.5);
                o.uv = center + ((_Offset + (v.uv - center)) / _Zoom);

                return o;
            }*/


            /*fixed4 frag (v2f i) : SV_Target
            {
                float2 clampedUV = clamp(i.uv, 0.001, 0.999); // Evita fugas en bordes
                fixed4 col = tex2D(_MainTex, clampedUV);
                fixed4 fog = tex2D(_FogTex, clampedUV);
                return col * (1 - fog.a);
            }*/
            fixed4 frag (v2f i) : SV_Target
            {
                float2 clampedUV = saturate(i.uv); // Alternativa eficiente a clamp
                fixed4 col = tex2D(_MainTex, clampedUV);
                fixed4 fog = tex2D(_FogTex, clampedUV);
                return col * (1 - fog.a);
            }
            ENDCG
        }
    }
}
