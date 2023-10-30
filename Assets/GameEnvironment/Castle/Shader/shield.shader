Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Colour", COLOR) = (0,0,0,0)
        _RippleStrength("Ripple Light Strength", float) = 1.0
        _RippleSpeed("Ripple Light Speed", float) = 1.0
        _FresnelIntensity("Fresnel Intensity", range(0,10))= 0
        _FresnelRamp("Fresnel Ramp", range(0,10))= 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha One

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            // these control the strength of ripple colour and 
            float _RippleStrength;
            float _RippleSpeed;
            // this controls the fresnel effect intensity, higher would result in brighter colour
            float _FresnelIntensity;
            // this controls the fresnel effect edge colour distance, higher would result in longer edge transition.
            float _FresnelRamp;


            v2f vert (appdata v)
            {
                v2f o;
            
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // Rotation Matrix
                float cosY = cos(_Time.y);
                float sinY = sin(_Time.y);
                float2x2 rot = float2x2(cosY, -sinY, sinY, cosY);
                o.uv = mul(rot, o.uv);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // this produce a colour change.
                fixed4 ripple = col * _Color * _RippleStrength * abs(sin(_Time.y * _RippleSpeed));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                // this creates a Fresnel effect around the effect
                float fresnelEffect = 1 - max(0,dot(i.normal, i.viewDir));
                fresnelEffect = pow(fresnelEffect, _FresnelRamp) * _FresnelIntensity;
                return fresnelEffect + ripple; 

                //return fixed4(col.rgb + ripple.rgb, col.a);
            }
            ENDCG
        }
    }
}
