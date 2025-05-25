using System;
using System.Collections;
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

  /// <summary>
  /// </summary>
  public ProceduralFairingConfig FairingConfig;


  /// Fairing material replacement
  private ModuleProceduralFairing fairingModule;
  private Material fairingMaterial;
  private Material fairingFlightMaterial;
  private Material fairingConeMaterial;
  private Material fairingFlightConeMaterial;

  public override void OnLoad(ConfigNode node)
  {
    base.OnLoad(node);

    if (Utils.IsLoadingPrefab())
    {
      // Loading prefab. Parse all zones in the part config.
      FairingConfig = new();
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

      // Get the fairing texture data, if it exists
      foreach (var fairingNode in node.GetNodes(Constants.MODULE_FAIRING_NODE))
      {
        try
        {
          FairingConfig = new(fairingNode);
        }
        catch (Exception e)
        {
          Utils.LogWarning($"{e.Message}");
        }
      }
    }

    Initialize(node.GetNodes(Constants.MODULE_COLOR_DATA_NODE));
  }

  public override void OnStart(StartState state)
  {
    base.OnStart(state);
    Initialize([]);

    /// Various special case handling for ModuleProceduralFairing
    StartCoroutine(DelaySetupFairingMaterials());
  }

  /// <summary>
  ///  This waits a tick, then terminates the stock fairing materials with extreme prejudice
  /// </summary>
  /// <returns></returns>
  private IEnumerator DelaySetupFairingMaterials()
  {
    yield return null;

    if (!(HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor))
    {
      yield break;
    }
    fairingModule = part.FindModuleImplementing<ModuleProceduralFairing>();
    if (fairingModule == null)
    {
      yield break;
    }

    /// For reference, stock shaders :
    /// editor: KSP/Bumped Specular (cutoff)
    /// flight: KSP/Bumped Specular Opaque (cutoff)

    /// Create the new fairing materials and replace them on the module
    CreateFairingMaterials();
    fairingModule.FairingMaterial = fairingMaterial;
    fairingModule.FairingConeMaterial = fairingConeMaterial;
    fairingModule.FairingFlightMaterial = fairingFlightMaterial;
    fairingModule.FairingFlightConeMaterial = fairingFlightConeMaterial;

    /// Replace existing panel materials
    List<Renderer> panelRenderers = new();
    foreach (ProceduralFairings.FairingPanel fairingPanel in fairingModule.Panels)
    {
      MeshRenderer mr = fairingPanel.go.GetComponent<MeshRenderer>();
      if (HighLogic.LoadedSceneIsEditor)
      {
        mr.material = fairingPanel.isCap ? fairingConeMaterial : fairingMaterial;
      }
      else
      {
        mr.material = fairingPanel.isCap ? fairingFlightConeMaterial : fairingFlightMaterial;
      }
      panelRenderers.Add(mr);
    }

    /// Check to see if a special config zone exists, if it doesn't we should create it
    string zoneName = Constants.FAIRING_ZONE_NAME;
    if (!ZoneData.TryGetValue(zoneName, out var zone))
    {
      zone = new(this, zoneName);

      /// Respect the editor's auto-apply option 
      if (HighLogic.LoadedSceneIsEditor)
      {
        var editorZoneData = TechnicolorEditorLogic.EditorData.GetZone(zoneName);
        if (editorZoneData.AutoApply) zone.SetSwatches(editorZoneData);
      }
      ZoneData[zoneName] = zone;
    }
    if (ZoneData.TryGetValue(zoneName, out zone))
    {
      zone.SetTargetRenderers(panelRenderers);
    }
    /// Need to hook the module up to onEditorShipModified, which gets fired when the user edits the fairing
    if (HighLogic.LoadedSceneIsEditor)
    {
      GameEvents.onEditorShipModified.Add(new EventData<ShipConstruct>.OnEvent(onEditorShipModified));
    }

    RefreshMPB();
  }

  /// <summary>
  /// Refresh the fairing renderers after a panel edit event and push them to the right zone
  /// </summary>
  private void RefreshFairingPanelRenderers()
  {
    if (fairingModule == null)
      return;

    List<Renderer> panelRenderers = new();
    foreach (ProceduralFairings.FairingPanel fairingPanel in fairingModule.Panels)
    {
      MeshRenderer mr = fairingPanel.go.GetComponent<MeshRenderer>();
      panelRenderers.Add(mr);
    }

    string zoneName = Constants.FAIRING_ZONE_NAME;
    if (ZoneData.TryGetValue(zoneName, out var zone))
    {
      zone.SetTargetRenderers(panelRenderers);
    }
    RefreshMPB();
  }

  private void onEditorShipModified(ShipConstruct ship)
  {
    RefreshFairingPanelRenderers();
  }

  private void CreateFairingMaterials()
  {
    Shader fairingTCShader = Shader.Find(Constants.TEAMCOLOR_FAIRING_SHADER_NAME);
    Shader fairingEditorTCShader = Shader.Find(Constants.TEAMCOLOR_FAIRING_EDITOR_SHADER_NAME);

    fairingMaterial = new(fairingEditorTCShader);
    fairingConeMaterial = new(fairingEditorTCShader);

    try
    {
      fairingMaterial.SetColor("_Color", Color.white);
      fairingMaterial.SetTexture("_MainTex",
        GameDatabase.Instance.GetTexture(FairingConfig.FairingAlbedoTexture, asNormalMap: false));
      fairingMaterial.SetTexture("_MetalMap",
        GameDatabase.Instance.GetTexture(FairingConfig.FairingMetalTexture, asNormalMap: false));
      fairingMaterial.SetTexture("_BumpMap",
        GameDatabase.Instance.GetTexture(FairingConfig.FairingNormalTexture, asNormalMap: false));
      fairingMaterial.SetTexture("_TeamColorMap",
        GameDatabase.Instance.GetTexture(FairingConfig.FairingTCTexture, asNormalMap: false));


      fairingConeMaterial.SetTexture("_MainTex",
        GameDatabase.Instance.GetTexture(FairingConfig.CapAlbedoTexture, asNormalMap: false));
      fairingConeMaterial.SetTexture("_MetalMap",
        GameDatabase.Instance.GetTexture(FairingConfig.CapMetalTexture, asNormalMap: false));
      fairingConeMaterial.SetTexture("_BumpMap",
        GameDatabase.Instance.GetTexture(FairingConfig.CapNormalTexture, asNormalMap: false));
      fairingConeMaterial.SetTexture("_TeamColorMap",
        GameDatabase.Instance.GetTexture(FairingConfig.CapTCTexture, asNormalMap: false));
    }
    catch (Exception e)
    {
      Utils.LogWarning($"Issue loading texture data for fairings: {e.Message}");
    }
    fairingFlightMaterial = new(fairingMaterial);
    fairingFlightMaterial.shader = fairingTCShader;
    fairingFlightConeMaterial = new(fairingConeMaterial);
    fairingFlightConeMaterial.shader = fairingTCShader;
  }

  public void Initialize(ConfigNode[] zoneDataNodes)
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

    FairingConfig ??= part.partInfo.partPrefab.Modules.GetModule<ModuleTechnicolor>().FairingConfig;
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
  public override void OnWillBeCopied(bool asSymCounterpart)
  {
    base.OnWillBeCopied(asSymCounterpart);
  }
  public override void OnWasCopied(PartModule copyPartModule, bool asSymCounterpart)
  {
    base.OnWasCopied(copyPartModule, asSymCounterpart);
    EditorData theseSwatches = new();
    this.GetPartSwatches(ref theseSwatches);
    ModuleTechnicolor targetModule = (ModuleTechnicolor)copyPartModule;

    targetModule.Initialize([]);
    targetModule.SetAllSwatches(theseSwatches, false);
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

  private void OnDestroy()
  {
    GameEvents.onEditorShipModified.Remove(onEditorShipModified);
  }
}
