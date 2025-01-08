using System;
using System.Collections.Generic;

namespace Technicolor
{
  [System.Serializable]
  public class EditorColorZone
  {
    public string Name => _name;
    public string DisplayName => _DisplayName;
    public bool AlwaysActive => _alwaysActive;
    public bool RestrictToGroupsDefault => _restrictToGroupsDefault;
    public string[] ValidGroups => _validGroups;

    private string _name;
    private string _DisplayName;
    private bool _alwaysActive = false;
    private bool _restrictToGroupsDefault = false;
    private string[] _validGroups;


    private const string NODE_NAME = "Name";
    private const string NODE_RESTRICT_TO_GROUP = "RestrictToGroupsDefault";
    private const string NODE_DISPLAYNAME = "DisplayName";
    private const string NODE_ALWAYSACTIVE = "AlwaysActive";
    private const string NODE_VALID_GROUP = "ValidGroup";

    public EditorColorZone()
    {

      _name = "default";
      _DisplayName = "Default";
      _restrictToGroupsDefault = false;
      _validGroups = new string[0];
    }
    public EditorColorZone(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue(NODE_NAME, ref _name);
      node.TryGetValue(NODE_RESTRICT_TO_GROUP, ref _restrictToGroupsDefault);
      node.TryGetValue(NODE_DISPLAYNAME, ref _DisplayName);
      node.TryGetValue(NODE_ALWAYSACTIVE, ref _alwaysActive);
      _validGroups = node.GetValues(NODE_VALID_GROUP);
    }

  }
  public class ZoneLibrary
  {
    public List<EditorColorZone> EditorColorZones;


    public ZoneLibrary() { }


    public void Load()
    {
      EditorColorZones = new();
      Utils.Log($"[ZoneLibrary] Loading Zones", LogType.Loading);
      foreach (var node in GameDatabase.Instance.GetConfigNodes(TechnicolorConstants.EDITORZONE_LIBRARY_CONFIG_NODE))
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
      foreach (EditorColorZone zone in EditorColorZones)
      {
        if (zone.Name == zoneName)
        {
          return zone.ValidGroups;
        }
      }

      return validGroups;
    }
  }
}
