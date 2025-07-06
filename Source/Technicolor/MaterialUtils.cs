using UnityEngine;

namespace Technicolor;

internal static class Extension
{
  public static bool IsEqualTo(this Color me, Color other)
  {
    return me.r == other.r && me.g == other.g && me.b == other.b && me.a == other.a;
  }
}

public static class MaterialUtils
{
  public static string[] ColorToStringArray(Color c)
  {
    string[] newArr = new string[4];
    newArr[0] = c.r.ToString();
    newArr[1] = c.g.ToString();
    newArr[2] = c.b.ToString();
    newArr[3] = c.a.ToString();
    return newArr;
  }

  public static void ApplyMaterials(ModuleTechnicolor zoneTemplate, GameObject rendererBase)
  {
    foreach (PartZoneData zone in zoneTemplate.ZoneData.Values)
    {

      foreach (var renderer in rendererBase.GetComponentsInChildren<Renderer>(true))
      {
        if (zone._configZone.Transforms.Contains(renderer.transform.name))
        {
          Material m = renderer.sharedMaterial;
          m.SetColor(ShaderPropertyID._TC1Color, zone.PrimarySwatch.Color);
          m.SetFloat(ShaderPropertyID._TC1Metalness, zone.PrimarySwatch.Metalness);
          m.SetFloat(ShaderPropertyID._TC1Smoothness, zone.PrimarySwatch.Smoothness);
          m.SetFloat(ShaderPropertyID._TC1SmoothBlend, zone.PrimarySwatch.SmoothBlend);
          m.SetFloat(ShaderPropertyID._TC1MetalBlend, zone.PrimarySwatch.MetalBlend);

          m.SetColor(ShaderPropertyID._TC2Color, zone.SecondarySwatch.Color);
          m.SetFloat(ShaderPropertyID._TC2Metalness, zone.SecondarySwatch.Metalness);
          m.SetFloat(ShaderPropertyID._TC2Smoothness, zone.SecondarySwatch.Smoothness);
          m.SetFloat(ShaderPropertyID._TC2SmoothBlend, zone.SecondarySwatch.SmoothBlend);
          m.SetFloat(ShaderPropertyID._TC2MetalBlend, zone.SecondarySwatch.MetalBlend);
        }
      }
    }

  }
}
