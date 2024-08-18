Shader "Resurfaced/PackedMetal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _MetalAlbedoMultiplier("Metal Albedo Multiplier",range(0,1.5)) = 1.0
        
        _MetalMap("Metal",2D) = "white" {}
        _Metalness ("Metalness", Range(0,1)) = 0.0
        _MetalRedMult("Metalness Red Multiplier", Range(0,1)) = 0.0
        _MetalGreenMult("Metalness Green Multiplier", Range(0,1)) = 0.0
        _MetalBlueMult("Metalness Blue Multiplier", Range(0,1)) = 0.0

        _BumpMap ("Bumpmap", 2D) = "bump" {}

        _EmissiveMap("Emission",2D) = "black" {}
        _EmissiveColor("Emission Color",Color) = (1,1,1,1)

        _RimColor("Rim Color",Color) = (1,1,1,1)
        _TemperatureColor("Temperature Color",Color) = (1,1,1,1)

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

            float3 viewDir;
        };

        half _Smoothness;
        half _MetalAlbedoMultiplier;
        half _Metalness;
        half _MetalRedMult;
        half _MetalGreenMult;
        half _MetalBlueMult;
        
        fixed4 _Color;
        fixed4 _EmissiveColor;

        fixed4 _RimColor;
        fixed4 _TemperatureColor;

        #define RIM_MULT 0.5

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D (_MetalMap, IN.uv_MainTex);
            fixed4 e = tex2D (_EmissiveMap, IN.uv_MainTex) * _EmissiveColor;

            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

            o.Metallic = saturate((m.r * _MetalRedMult) +  (m.g * _MetalGreenMult) + (m.b * _MetalBlueMult)) * _Metalness;
            o.Albedo = lerp(c.rgb,(_MetalAlbedoMultiplier * c.rgb),o.Metallic) * IN.color.rgb;
            o.Smoothness = c.a * _Smoothness;
            o.Emission = (e * e.a) + (_RimColor.rgb * rim * RIM_MULT) + (_TemperatureColor.rgb * _TemperatureColor.a);
            o.Alpha = c.a;
            o.Normal = UnpackNormalDXT5nm (tex2D (_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
