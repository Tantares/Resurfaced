Shader "Resurfaced/Standard"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 1.0
        _MetalAlbedoMultiplier("Metal Albedo Multiplier",range(0,1.5)) = 1.0
        
        _MetalMap("Metal",2D) = "white" {}
        _Metalness ("Metalness", Range(0,1)) = 1.0

        _BumpMap ("Bumpmap", 2D) = "normal" {}

        _Emissive("Emission",2D) = "black" {}
        _EmissiveColor("Emission Color",Color) = (0,0,0,0)

        _RimColor("Rim Color",Color) = (0,0,0,0)
        _TemperatureColor("Temperature Color",Color) = (0,0,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        CGPROGRAM

        #include "LightingKSP.cginc"

        #pragma surface surf StandardKSP fullforwardshadows
        #pragma target 3.0


        sampler2D _MainTex;
        sampler2D _MetalMap;
        sampler2D _BumpMap;
        sampler2D _Emissive;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Emissive;
            float2 uv_BumpMap;

            half4 color : COLOR;

            float3 viewDir;
        };

        half _Smoothness;
        half _MetalAlbedoMultiplier;
        half _Metalness;
        
        fixed4 _EmissiveColor;

        fixed4 _RimColor;
        fixed4 _TemperatureColor;

        #define RIM_MULT 0.5

        #define ALBEDO_RAMP_A 0.25
        #define ALBEDO_RAMP_B 0.1
        #define ALBEDO_RAMP_DIVIDER 0.16

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D (_MetalMap, IN.uv_MainTex);
            fixed4 e = tex2D (_Emissive, IN.uv_Emissive) * _EmissiveColor;

            fixed cl = Luminance(c.rgb);
            fixed ol = cl;
            cl = ((cl - ALBEDO_RAMP_B) * (ALBEDO_RAMP_A - (ALBEDO_RAMP_B * cl))) / ALBEDO_RAMP_DIVIDER;

            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));

            o.Albedo = lerp(c.rgb,(_MetalAlbedoMultiplier * c.rgb),m.r) * IN.color.rgb;

            fixed isScaled = step(o.Albedo, 0.25) * (1.0 - m.r);
            fixed albedoMult = isScaled * max(0, cl) / max(1e-3, ol) + (1 - isScaled); 

            o.Albedo *= (albedoMult);

            o.Metallic = m.r * _Metalness;
            o.Smoothness = c.a * _Smoothness;
            o.Emission = (e * e.a) + (_RimColor.rgb * rim * RIM_MULT * _RimColor.a) + (_TemperatureColor.rgb * _TemperatureColor.a);
            o.Alpha = c.a;
            o.Normal = UnpackNormalDXT5nm (tex2D (_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
