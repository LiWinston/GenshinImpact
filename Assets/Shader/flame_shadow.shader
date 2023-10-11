Shader "Unlit/testing"
{
    Properties  
    {
        _MainTex ("Albedo Texture", 2D) = "None" {}
        _GradientTex("Gradient", 2D) = "White" {}
        _TintColor("Tint Color", Color) = (1,1,1,1)
        _Transparency("Transparency", Range(0.0,1.0)) = 0.75
        _Distance("Distance", Float) = 1
        _Amplitude("Amplitude", Float) = 1
        _Speed("Speed", Float) = 1
        _Amount("Amount", Float) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION; 
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _GradientTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float _Transparency;
            float _Distance;
            float _Amplitude;
            float _Speed;
            float _Amount;

            v2f vert (appdata v)
            {
                v2f o;
                // moves up and down, by sin,
                _Speed += 0.2*sin(_Time);
                v.vertex.y += tan(_Time.y * _Speed) * _Distance * _Amount;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {             

                float gray = tex2D(_MainTex, i.uv).r;
				// get scrolling        
				float scroll = frac(gray + _Time.x*_Speed);
                // sample the texture
                fixed4 col = tex2D(_GradientTex, float2(scroll,1.5));
                col.a = _Transparency;
                return col;
            }
            ENDCG
        }
    }
}
