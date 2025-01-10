using System.Collections.Generic;

namespace Technicolor;

public static class ZoneLibrary
{
  public static readonly Dictionary<string, EditorColorZone> EditorColorZones = new();

  public static void Load()
  {
    Utils.Log($"[ZoneLibrary] Loading Zones", LogType.Loading);
    foreach (var node in GameDatabase.Instance.GetConfigNodes(
               Constants.EDITORZONE_LIBRARY_CONFIG_NODE))
    {
      foreach (var subNode in node.GetNodes(Constants.EDITORZONE_CONFIG_NODE))
      {
        try
        {
          EditorColorZone cz = new(subNode);
          EditorColorZones[cz.Name] = cz;
        }
        catch
        {
          Utils.LogError($"[ZoneLibrary] Issue loading preset from node: {subNode}");
        }
      }
    }

    Utils.Log($"[ZoneLibrary] Loaded {EditorColorZones.Count} zones", LogType.Loading);
  }

  public static string[] GetValidGroupsForZone(string zoneName)
  {
    if (EditorColorZones.TryGetValue(zoneName, out var zone))
    {
      return zone.ValidGroups;
    }

    Utils.LogWarning($"[ZoneLibrary] Couldn't find {zoneName} in defined zones");
    return [];
  }

  public static string GetZoneDisplayName(string zoneName)
  {
    if (EditorColorZones.TryGetValue(zoneName, out var zone))
    {
      return zone.DisplayName;
    }

    Utils.LogWarning($"[ZoneLibrary] Couldn't find {zoneName} in defined zones");
    return zoneName;
  }
}
