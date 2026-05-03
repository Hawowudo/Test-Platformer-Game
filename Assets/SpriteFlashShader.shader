Shader "Unlit/SpriteFlashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
        _FlashAmount ("Flash Amount", Range(0,1)) = 0
        _Test("test", float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct vertexToFragment
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _FlashColor;
            float _FlashAmount;

            vertexToFragment vert (appdata v)
            {
                vertexToFragment o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (vertexToFragment i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // apply flash on top
                col.rgb = lerp(col.rgb, _FlashColor.rgb, _FlashAmount * _FlashColor.a);

                return col;
            }
            ENDCG
        }
    }
}