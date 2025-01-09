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

  /// Taken from https://github.com/al2me6/AdaptiveTanks/blob/7495d186b5594e08182b58cb6abcfaf479f5a971/Src/AdaptiveTanks/Utils/PartUtils.cs#L35-L65
  public record PartMPBProperties
  {
    public float Opacity { get; set; }
    public float? RimFalloff { get; set; }
    public Color RimColor { get; set; }
    public Color? TemperatureColor { get; set; }

    public void WriteTo(ref MaterialPropertyBlock mpb)
    {
      mpb.SetFloat(PropertyIDs._Opacity, Opacity);
      if (RimFalloff != null)
        mpb.SetFloat(PropertyIDs._RimFalloff, RimFalloff.Value);
      mpb.SetColor(PropertyIDs._RimColor, RimColor);
      if (TemperatureColor != null)
        mpb.SetColor(PhysicsGlobals.TemperaturePropertyID, TemperatureColor.Value);
    }
  }

  public static void ExtractMPBProperties(this Part part, ref PartMPBProperties props)
  {
    float opacity = part.mpb.GetFloat(PropertyIDs._Opacity);
    props.Opacity = opacity > 0f ? opacity : 1f;

    float rimFalloff = part.mpb.GetFloat(PropertyIDs._RimFalloff);
    props.RimFalloff = rimFalloff != 0f ? rimFalloff : null;

    props.RimColor = part.mpb.GetColor(PropertyIDs._RimColor);

    props.TemperatureColor = HighLogic.LoadedSceneIsFlight
      ? part.mpb.GetColor(PhysicsGlobals.TemperaturePropertyID)
      : null;
  }
}
