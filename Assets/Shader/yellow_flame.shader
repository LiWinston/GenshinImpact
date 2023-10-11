Shader "Unlit/yellow_flame"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Colour", color) = (1,1,1,1)
        //_Transparency("Transparency", Range(0.0,1.0)) = 0.75
        _Distance("Distance", Float) = 1
        _Amplitude("Amplitude", Float) = 1
        _Speed("Speed", Float) = 1
        _Amount("Amount", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Transparency;
            float _Distance;
            float _Amplitude;
            float _Speed;
            float _Amount;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                //shift position
                v.vertex.y += cos(_Time.y * _Speed) * _Distance * _Amount;
                v.vertex.x += cos(_Time.x * _Speed) * _Distance * _Amount;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                return col;
            }
            ENDCG
        }
    }
}
