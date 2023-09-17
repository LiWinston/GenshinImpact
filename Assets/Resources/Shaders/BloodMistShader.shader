// BloodMistShader.shader

Shader "BloodMistShader"
{
    Properties
    {
        _PlayerHealth ("Player Health", Range(0, 1)) = 1
        _BloodColor ("Blood Color", Color) = (1, 0, 0, 0.65) // 默认为红色
        _FadeOutSpeed ("Fade Out Speed", Range(0.1, 10)) = 0.4 // 淡出速度
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
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
            };
            
            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };
            
            float _PlayerHealth;
            float4 _BloodColor;
            float _FadeOutSpeed;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _BloodColor;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                half playerHealth = _PlayerHealth;
                float mistDensity = 1.0 - playerHealth;
                half4 mistColor = half4(1, 0, 0, 1); // 红色
                half4 finalColor = lerp(mistColor, i.color, mistDensity);

                // 应用淡出效果
                finalColor.a *= exp(-_FadeOutSpeed * _Time.y);

                return finalColor;
            }
            ENDCG
        }
    }
}
