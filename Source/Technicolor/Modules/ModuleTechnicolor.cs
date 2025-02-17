using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Technicolor;

public class ModuleTechnicolor : PartModule
{
  /// <summary>
  /// Stores readonly zone information specified in the part config. Only parsed during prefab
  /// compilation; the prefab data is referenced by instanced parts.
  /// </summary>
  public Dictionary<string, ColorZone> ConfigZones = null;

  /// <summary>
  /// Stores the user-selected swatches for each zone. Only populated by instantiated parts and
  /// is serialized to the vessel. If the serialized data is empty in `OnLoad`, the part is freshly
  /// instantiated and the default swatch selections are applied.
  /// </summary>
  public readonly Dictionary<string, PartZoneData> ZoneData = [];

  public override void OnLoad(ConfigNode node)
  {
    base.OnLoad(node);

    if (Utils.IsLoadingPrefab())
    {
      // Loading prefab. Parse all zones in the part config.
      ConfigZones = new();

      foreach (var zoneNode in node.GetNodes(Constants.MODULE_COLOR_NODE))
      {
        try
        {
          ColorZone configZone = new(zoneNode);
          ConfigZones[configZone.Name] = configZone;
        }
        catch (Exception e)
        {
          Utils.LogWarning($"{e.Message}");
        }
      }

      if (ConfigZones.Count == 0)
      {
        ColorZone defaultZone = new();
        ConfigZones[defaultZone.Name] = defaultZone;
      }
    }

    Initialize(node.GetNodes(Constants.MODULE_COLOR_DATA_NODE));
  }

  public override void OnStart(StartState state)
  {
    base.OnStart(state);
    Initialize([]);
  }

  private void Initialize(ConfigNode[] zoneDataNodes)
  {
    // Initialization may occur either in `OnLoad` or `OnStart`. Initialize only once.
    if (ZoneData.Count > 0) return;

    // If there are no config zones, pull them from the prefab.
    ConfigZones ??= part.partInfo.partPrefab.Modules.GetModule<ModuleTechnicolor>().ConfigZones;

    var savedZones = zoneDataNodes
      .Select(dataNode => new PartZoneData(this, dataNode))
      .ToDictionary(zone => zone.Name);

    // Initialize zones from the saved data as much as possible.
    // Try to handle cases where the zones in the config have changed gracefully.
    foreach (string zoneName in ConfigZones.Keys)
    {
      if (!savedZones.TryGetValue(zoneName, out var zone))
      {
        // Create a new zone from the part config if one was not found.
        zone = new(this, zoneName);

        // Respect the editor's auto-apply option if possible.
        if (HighLogic.LoadedSceneIsEditor)
        {
          var editorZoneData = TechnicolorEditorLogic.EditorData.GetZone(zoneName);
          if (editorZoneData.AutoApply) zone.SetSwatches(editorZoneData);
        }
      }

      zone.FindTargetRenderers();
      ZoneData[zoneName] = zone;
    }
    RefreshMPB();
  }

  public void LateUpdate()
  {
    RefreshMPB();
  }

  public override void OnSave(ConfigNode node)
  {
    base.OnSave(node);

    foreach (var zone in ZoneData.Values)
    {
      var zoneDataNode = new ConfigNode(Constants.MODULE_COLOR_DATA_NODE);
      zone.Save(zoneDataNode);
      node.AddNode(zoneDataNode);
    }
  }

  public void SetAllSwatches(EditorData editorData, bool updateCounterparts = true)
  {
    foreach (var editorZoneData in editorData.Zones)
    {
      if (editorZoneData.ActiveInEditor) SetZoneSwatches(editorZoneData);
    }
    if (updateCounterparts)
    {
      foreach (Part p in part.symmetryCounterparts)
      {
        p.GetComponent<ModuleTechnicolor>()?.SetAllSwatches(editorData, false);
      }
    }

  }

  public void SetZoneSwatches(EditorZoneData data)
  {
    if (!ZoneData.ContainsKey(data.Name)) return;
    Utils.Log($"[ModuleTechnicolor] Applying swatches for zone {data.Name} To part",
              LogType.Editor);
    ZoneData[data.Name].SetSwatches(data);
  }

  public void GetPartSwatches(ref EditorData editorData)
  {
    foreach (var editorZoneData in editorData.Zones)
    {
      if (!ZoneData.TryGetValue(editorZoneData.Name, out var partZone))
      {
        editorZoneData.ActiveInEditor = editorZoneData.AlwaysActive;
        continue;
      }

      Utils.Log(
        $"[ModuleTechnicolor] Sampling {partZone.PrimarySwatch.Name} and {partZone.SecondarySwatch.Name} from {partZone.Name}",
        LogType.Editor);
      editorZoneData.ActiveInEditor = true;
      editorZoneData.PrimarySwatch = partZone.PrimarySwatch;
      editorZoneData.SecondarySwatch = partZone.SecondarySwatch;
    }
  }

  private MaterialPropertyBlock mpb;
  private MaterialUtils.PartMPBProperties partMPBProps = new();

  protected void RefreshMPB()
  {
    if (mpb == null) mpb = new();

    mpb.Clear();
    part.ExtractMPBProperties(ref partMPBProps);
    partMPBProps.WriteTo(ref mpb);

    foreach (var zone in ZoneData.Values) zone.Apply(mpb);
  }
}
