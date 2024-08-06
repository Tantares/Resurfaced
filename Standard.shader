Shader "Resurfaced/Standard"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _MetalAlbedoMultiplier("Metal Albedo Multiplier",range(0,1.5)) = 1.0
        
        _MetalMap("Metal",2D) = "white" {}
        _Metalness ("Metalness", Range(0,1)) = 0.0

        _BumpMap ("Bumpmap", 2D) = "bump" {}

        _EmissiveMap("Emission",2D) = "black" {}
        _EmissiveColor("Emission Color",Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MetalMap;
        sampler2D _BumpMap;
        sampler2D _EmissiveMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;

            half4 color : COLOR;
        };

        half _Smoothness;
        half _MetalAlbedoMultiplier;
        half _Metalness;
        
        fixed4 _Color;
        fixed4 _EmissiveColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D (_MetalMap, IN.uv_MainTex);
            fixed4 e = tex2D (_EmissiveMap, IN.uv_MainTex) * _EmissiveColor;

            o.Albedo = lerp(c.rgb,(_MetalAlbedoMultiplier * c.rgb),m.r) * IN.color.rgb;
            o.Metallic = m.r * _Metalness;
            o.Smoothness = c.a * _Smoothness;
            o.Emission = e * e.a;
            o.Alpha = c.a;
            o.Normal = UnpackNormalDXT5nm (tex2D (_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
