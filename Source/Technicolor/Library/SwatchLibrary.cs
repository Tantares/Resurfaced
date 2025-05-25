using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technicolor;

public static class SwatchLibrary
{
  public static readonly Swatch DefaultSwatch = new();
  public static readonly List<Swatch> Swatches = [DefaultSwatch];
  public static readonly Dictionary<string, SwatchGroup> SwatchGroups = new();

  private static readonly Dictionary<string, Swatch> _swatchNameCache = new();

  public static bool Has(string name)
  {
    if (String.IsNullOrEmpty(name)) return false;

    if (_swatchNameCache.ContainsKey(name)) return true;

    if (Swatches.Find(s => s.Name == name) is Swatch sw)
    {
      _swatchNameCache[name] = sw;
      return true;
    }

    return false;
  }

  public static Swatch Get(string name)
  {
    if (Has(name)) return _swatchNameCache[name];

    Utils.Log($"[SwatchLibrary] swatch {name} could not be found", LogType.Any);
    return DefaultSwatch;
  }

  public static void Load()
  {
    Utils.Log($"[SwatchLibrary] Loading Swatches", LogType.Loading);
    foreach (var node in GameDatabase.Instance.GetConfigNodes(Constants.SWATCH_LIBRARY_CONFIG_NODE))
    {
      foreach (var subNode in node.GetNodes(Constants.SWATCH_CONFIG_NODE))
      {
        try
        {
          Swatches.Add(new(subNode));
        }
        catch
        {
          Utils.LogError($"[SwatchLibrary] Issue swatch preset from node: {subNode}");
        }
      }
    }

    IComparer<Swatch> comparer = new TechnicolorSwatchComparer();
    Swatches.Sort(comparer);

    Utils.Log($"[SwatchLibrary] Loaded {Swatches.Count} Swatches", LogType.Loading);
    foreach (var node in GameDatabase.Instance.GetConfigNodes(Constants.GROUP_LIBRARY_CONFIG_NODE))
    {
      foreach (var subNode in node.GetNodes(Constants.GROUP_CONFIG_NODE))
      {
        try
        {
          SwatchGroup sg = new(subNode);
          SwatchGroups[sg.Name] = sg;
        }
        catch
        {
          Utils.LogError($"[SwatchLibrary] Issue loading group from node: {subNode}");
        }
      }
    }

    Utils.Log($"[SwatchLibrary] Loaded {SwatchGroups.Count} Swatch Groups", LogType.Loading);
  }


  public class TechnicolorSwatchComparer : IComparer<Swatch>
  {
    public float[] groupedHues = new float[] { 13/360f, 37/360f, 66/360f, 140/360f, 178/360f, 264/360f, 329/360f};

    public int Compare(Swatch x, Swatch y)
    {
      Color.RGBToHSV(x.Color, out float h1, out float s1, out float v1);
      Color.RGBToHSV(y.Color, out float h2, out float s2, out float v2);

      // compare smoothnesses, if different, use this
      int compareSmooth = x.Smoothness.CompareTo(y.Smoothness);
      if (compareSmooth != 0) return compareSmooth;

      // if saturation is low, compare values
      if (s1 == 0f || s2 == 0f)
        return v1.CompareTo(v2);

      // Else do a fancy hue/value thing
      // Bin hues for comparison
      int hueBin1 = 0;
      int huebin2 = 0;
      for (int i = 0; i < groupedHues.Length; i++)
      {
        if (h1 <= groupedHues[i])
        {
          hueBin1 = i;
          break;
        }
      }
      ///  // Intense : H:X, S:80 , V:100, 
      // Dark    : H:X, S:80: , V:35, 
      // Medium  : H:X, S:75: , V:85, 
      for (int i = 0; i < groupedHues.Length; i++)
      {
        if (h2 <= groupedHues[i])
        {
          huebin2 = i;
          break;
        }
      }

      int compareHueBin = hueBin1.CompareTo(huebin2);

      if (compareHueBin != 0) return compareHueBin;

      int compareValue = v1.CompareTo(v2);
      return compareValue;
    }
  }
}
