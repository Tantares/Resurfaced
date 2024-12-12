using System;
using System.Collections.Generic;

namespace Technicolor
{
  public class SwatchLibrary
  {
    public List<TechnicolorSwatch> Swatches;
    public List<String> SwatchGroups;

    public TechnicolorSwatch DefaultSwatch;

    public SwatchLibrary() { }
    public bool HasSwatch(string name)
    {
      return Swatches.Find(x => x.Name == name) != null;
    }

    public TechnicolorSwatch GetSwatch(string name)
    {
      TechnicolorSwatch sw = Swatches.Find(x => x.Name == name);
      if (sw != null)
      {
        return sw;
      }
      Utils.Log($"[SwatchLibrary] swatch {name} could not be found", LogType.Any);
      return null;
    }

    public void Load()
    {
      Swatches = new();
      SwatchGroups = new();
      DefaultSwatch = new();
      Swatches.Add(DefaultSwatch);
      Utils.Log($"[SwatchLibrary] Loading Swatches", LogType.Loading);
      foreach (var node in GameDatabase.Instance.GetConfigNodes(TechnicolorConstants.SWATCH_LIBRARY_CONFIG_NODE))
      {
        foreach (var subNode in node.GetNodes(TechnicolorConstants.SWATCH_CONFIG_NODE))
        {
          try
          {
            Swatches.Add(new(subNode));
          }
          catch
          {
            Utils.LogError($"[SwatchLibrary] Issue loading preset from node: {subNode}");
          }
        }
      }
      Utils.Log($"[SwatchLibrary] Loaded {Swatches.Count} Swatches", LogType.Loading);
      foreach (TechnicolorSwatch swatch in Swatches)
      {
        if (!SwatchGroups.Contains(swatch.Group) && swatch.Group != "" && swatch.Group != "internal")
        {
          SwatchGroups.Add(swatch.Group);
        }
      }
      Utils.Log($"[SwatchLibrary] Loaded {SwatchGroups.Count} Swatch Groups", LogType.Loading);
    }

    
  }
}
