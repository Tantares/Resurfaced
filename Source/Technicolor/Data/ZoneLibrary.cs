using System.Collections.Generic;

namespace Technicolor;

public class ZoneLibrary
{
  public List<EditorColorZone> EditorColorZones;

  public ZoneLibrary() { }

  public void Load()
  {
    EditorColorZones = new();
    Utils.Log($"[ZoneLibrary] Loading Zones", LogType.Loading);
    foreach (var node in GameDatabase.Instance.GetConfigNodes(
               TechnicolorConstants.EDITORZONE_LIBRARY_CONFIG_NODE))
    {
      foreach (var subNode in node.GetNodes(TechnicolorConstants.EDITORZONE_CONFIG_NODE))
      {
        try
        {
          EditorColorZones.Add(new(subNode));
        }
        catch
        {
          Utils.LogError($"[ZoneLibrary] Issue loading preset from node: {subNode}");
        }
      }
    }

    Utils.Log($"[ZoneLibrary] Loaded {EditorColorZones.Count} zones", LogType.Loading);
  }

  public string[] GetValidGroupsForZone(string zoneName)
  {
    string[] validGroups = new string[0];
    foreach (var zone in EditorColorZones)
    {
      if (zone.Name == zoneName)
      {
        return zone.ValidGroups;
      }
    }

    return validGroups;
  }

  public string GetZoneDisplayName(string zoneName)
  {
    foreach (var zone in EditorColorZones)
    {
      if (zone.Name == zoneName)
      {
        return zone.DisplayName;
      }
    }

    Utils.LogWarning($"[ZoneLibrary] Couldn't find {zoneName} in defined zones");
    return zoneName;
  }
}
