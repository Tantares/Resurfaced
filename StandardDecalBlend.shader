Shader "Resurfaced/StandardDecalBlend"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        
        _MetalMap("Metal",2D) = "white" {}
        _Metalness ("Metallic", Range(0,1)) = 0.0

        _BumpMap ("Bumpmap", 2D) = "bump" {}

        _EmissionMap("Emission",2D) = "black" {}
        _EmissionColor("Emission Color",Color) = (1,1,1,1) 
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" 
            "IgnoreProjector" = "True" 
            "Queue" = "Transparent" 
        }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows decal:blend
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MetalMap;
        sampler2D _BumpMap;
        sampler2D _EmissionMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;

            half4 color : COLOR;
        };

        half _Smoothness;
        half _Metalness;
        
        fixed4 _Color;
        fixed4 _EmissionColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D (_MetalMap, IN.uv_MainTex);
            fixed4 e = tex2D (_EmissionMap, IN.uv_MainTex) * _EmissionColor;

            o.Albedo = c.rgb * IN.color.rgb;
            o.Metallic = m * _Metalness;
            o.Smoothness = c.a * _Smoothness;
            o.Emission = e;
            o.Alpha = c.a;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
