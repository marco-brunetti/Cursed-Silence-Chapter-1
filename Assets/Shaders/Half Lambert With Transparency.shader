Shader "Custom/Half Lambert With Transparency" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _WrapAmount ("Wrap Amount", Range (0.0, 1.0)) = 0.5
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Enable alpha blending
        ZWrite Off // Disable depth writing for transparency
        
        CGPROGRAM
        #pragma surface surf WrapLambert alpha:fade
        
        float _WrapAmount;
        fixed4 _Color; // Define the color variable

        half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = dot (s.Normal, lightDir);
            half diff = NdotL * _WrapAmount + (1 - _WrapAmount);
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
            c.a = s.Alpha; // Maintain alpha value
            return c;
        }

        struct Input {
            float2 uv_MainTex;
        };
        sampler2D _MainTex;

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = texColor.rgb * _Color.rgb; // Multiply by the color
            o.Alpha = texColor.a * _Color.a;      // Use the texture and color alpha values
        }
        ENDCG
    }
    Fallback "Transparent/Cutout/VertexLit"
}
