using System.Collections.Generic;
using UnityEngine;

namespace Technicolor;

public static class SwatchLibrary
{
  public static readonly Swatch DefaultSwatch = new();
  public static readonly List<Swatch> Swatches = [DefaultSwatch];
  public static readonly Dictionary<string, SwatchGroup> SwatchGroups = new();

  private static readonly Dictionary<string, Swatch> _swatchNameCache = new();

  public static bool HasSwatch(string name)
  {
    if (_swatchNameCache.ContainsKey(name))
    {
      return true;
    }

    if (Swatches.Find(s => s.Name == name) is Swatch sw)
    {
      _swatchNameCache.Add(name, sw);
      return true;
    }

    return false;
  }

  public static Swatch GetSwatch(string name)
  {
    if (HasSwatch(name))
    {
      return _swatchNameCache[name];
    }

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
          SwatchGroups.Add(sg.Name, sg);
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
    public int Compare(Swatch x, Swatch y)
    {
      Color.RGBToHSV(x.Color, out float h1, out float _, out float _);
      Color.RGBToHSV(y.Color, out float h2, out float _, out float _);

      int compareSmooth = x.Smoothness.CompareTo(y.Smoothness);
      if (compareSmooth != 0)
      {
        return compareSmooth;
      }

      int compareHue = h1.CompareTo(h2);
      return compareHue;
    }
  }
}
