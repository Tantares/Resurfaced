Shader "Resurfaced/Standard (TC)"
{
  Properties
  {
      _Color("Color", Color) = (1,1,1,1)

      _MainTex("Albedo (RGB)", 2D) = "white" {}
      _Smoothness("Smoothness", Range(0,1)) = 1.0
      _MetalAlbedoMultiplier("Metal Albedo Multiplier",range(0,1.5)) = 1.0

      _MetalMap("Metal",2D) = "white" {}
      _Metalness("Metalness", Range(0,1)) = 1.0

      _BumpMap("Bumpmap", 2D) = "normal" {}

      _Emissive("Emission",2D) = "black" {}
      _EmissiveColor("Emission Color",Color) = (0,0,0,0)


      _TeamColorMap("Team Color",2D) = "black" {}
      _TC1Color("TC 1",Color) = (0,0,0,0)
      _TC1Metalness("TC 1 Metal",Range(0,1)) = 1.0
      _TC1Smoothness("TC 1 Smooth",Range(0,1)) = 1.0


      _TC1MetalBlend("TC 1 Metal Blend",Range(0,1)) = 0.0
      _TC1SmoothBlend("TC 1 Smooth Blend",Range(0,1)) = 0.0

      _TC2Color("TC 2",Color) = (0,0,0,0)
      _TC2Metalness("TC 2 Metal",Range(0,1)) = 1.0
      _TC2Smoothness("TC 2 Smooth",Range(0,1)) = 1.0


      _TC2MetalBlend("TC 2 Metal Blend",Range(0,1)) = 0.0
      _TC2SmoothBlend("TC 2 Smooth Blend",Range(0,1)) = 0.0

      _RimColor("Rim Color",Color) = (0,0,0,0)
      _TemperatureColor("Temperature Color",Color) = (0,0,0,0)

  }
    SubShader
      {
          Tags { "RenderType" = "Opaque" }
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
          sampler2D _TeamColorMap;

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

          fixed4 _TC1Color;
          half _TC1Smoothness;
          half _TC1Metalness;

          half _TC1MetalBlend;
          half   _TC1SmoothBlend;

          fixed4 _TC2Color;
          half _TC2Smoothness;
          half _TC2Metalness;

          half _TC2MetalBlend;
          half _TC2SmoothBlend;
          fixed4 _EmissiveColor;

          fixed4 _RimColor;
          fixed4 _TemperatureColor;

          #define RIM_MULT 0.5

          #define ALBEDO_RAMP_A 0.25
          #define ALBEDO_RAMP_B 0.1
          #define ALBEDO_RAMP_DIVIDER 0.16

          void surf(Input IN, inout SurfaceOutputStandard o)
          {
              fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
              fixed4 m = tex2D(_MetalMap, IN.uv_MainTex);
              fixed4 e = tex2D(_Emissive, IN.uv_Emissive) * _EmissiveColor;

              fixed4 tc = tex2D(_TeamColorMap, IN.uv_MainTex);

              fixed cl = Luminance(c.rgb);
              fixed ol = cl;
              cl = ((cl - ALBEDO_RAMP_B) * (ALBEDO_RAMP_A - (ALBEDO_RAMP_B * cl))) / ALBEDO_RAMP_DIVIDER;

              half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
              fixed3 cFinal = lerp(c.rgb, (_MetalAlbedoMultiplier * c.rgb), m.r) * IN.color.rgb;

              half gray = (cFinal.r + cFinal.b + cFinal.g) / 3.0;
              half threshold = step(gray, 0.5);

              fixed3 multiply = 2.0 * cFinal * _TC1Color;
              fixed3 screen = 1.0 - (2.0 * (1.0 - cFinal)) * (1.0 - _TC1Color);
              fixed3 overlayR = (multiply * (1.0 - threshold)) + (screen * threshold);

              multiply = 2.0 * cFinal * _TC2Color;
              screen = 1.0 - (2.0 * (1.0 - cFinal)) * (1.0 - _TC2Color);
              fixed3 overlayB = (multiply * (1.0 - threshold)) + (screen * threshold);
              o.Albedo = lerp(cFinal, overlayR, tc.r * _TC1Color.a);
              o.Albedo = lerp(o.Albedo, overlayB, tc.g * _TC2Color.a);

              o.Metallic = lerp(m.r * _Metalness, lerp(m.r * _TC1Metalness,tc.a * _TC1Metalness ,_TC1MetalBlend), tc.r * _TC1Color.a);
              o.Metallic = lerp(o.Metallic, lerp(m.r * _TC2Metalness, tc.a * _TC2Metalness, _TC2MetalBlend), tc.g * _TC2Color.a);

              o.Smoothness = lerp(c.a * _Smoothness, lerp(c.a * _TC1Smoothness, tc.b * _TC1Smoothness, _TC1SmoothBlend), tc.r * _TC1Color.a);
              o.Smoothness = lerp(o.Smoothness, lerp(c.a * _TC2Smoothness, tc.b * _TC2Smoothness, _TC2SmoothBlend), tc.g * _TC2Color.a);

              o.Emission = (e * _EmissiveColor.a) + (_RimColor.rgb * rim * RIM_MULT * _RimColor.a) + (_TemperatureColor.rgb * _TemperatureColor.a);
              o.Alpha = c.a;
              o.Normal = UnpackNormalDXT5nm(tex2D(_BumpMap, IN.uv_BumpMap));
          }
          ENDCG
      }
        FallBack "Diffuse"
}
