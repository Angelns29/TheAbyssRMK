Shader "Custom/MinimapFog"
{
    Properties
    {
        _MinimapTex ("Minimap Texture", 2D) = "white" {}
        _FogTex ("Fog Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            sampler2D _MinimapTex;
            sampler2D _FogTex;
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 minimapCol = tex2D(_MinimapTex, i.uv);
                fixed4 fogCol = tex2D(_FogTex, i.uv);
                return minimapCol * (1.0 - fogCol.a);
            }
            ENDCG
        }
    }
}
