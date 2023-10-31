Shader "Unlit/testing"
{
    Properties  
    {
        _MainTex ("Albedo Texture", 2D) = "None" {}
        _GradientTex("Gradient", 2D) = "White" {}
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
            sampler2D _GradientTex;
            float4 _MainTex_ST;
            float _Transparency;
            float _Distance;
            float _Amplitude;
            float _Speed;
            float _Amount;

            v2f vert (appdata v)
            {
                v2f o;
                // this cause flame shadow to occillate.
                v.vertex.y += tan(_Time.y * _Speed) * _Distance * _Amount;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {                     
                // this using the colour gradient   
                float gray = tex2D(_MainTex, i.uv).r;
				// this gets a gradual colour change over time, controlled by speed.        
				float change_colour = frac(gray + _Time.x*_Speed);
                // this samples the gradient colour from the colour gradient to apply to the main texture.
                fixed4 col = tex2D(_GradientTex, float2(change_colour,0.8));
                // apply transparency to the object
                col.a = _Transparency;
                return col;
            }
            ENDCG
        }
    }
}
