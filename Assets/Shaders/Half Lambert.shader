Shader "Custom/Half Lambert" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _WrapAmount ("Wrap Amount", Range (0.0, 1.0)) = 0.5
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf WrapLambert
        
        float _WrapAmount;
        fixed4 _Color; // Define the color variable

        half4 LightingWrapLambert (SurfaceOutput s, half3 lightDir, half atten) {
            half NdotL = dot (s.Normal, lightDir);
            half diff = NdotL * _WrapAmount + (1 - _WrapAmount);
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
            c.a = s.Alpha;
            return c;
        }

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };
        sampler2D _MainTex;
        sampler2D _BumpMap;

        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color.rgb; // Multiply by the color
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    Fallback "Diffuse"
}
