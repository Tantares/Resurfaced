namespace Technicolor;

public class TechnicolorPersistentZoneData
{
  public string ZoneName = "main";
  public string DisplayName = "Main";
  public TechnicolorSwatch PrimarySwatch;
  public TechnicolorSwatch SecondarySwatch;

  public bool RestrictToMaterialGroups = true;
  public bool AutoApply = false;
  public bool ActiveInEditor = false;
  public bool AlwaysActive = false;

  private const string NODE_NAME = "SmoothBlend";
  private const string NODE_PRIMARY = "PrimarySwatch";
  private const string NODE_SECONDARY = "SecondarySwatch";
  private const string NODE_ALWAYSACTIVE = "AlwaysActive";
  private const string NODE_RESTRICT_SWATCHES = "RestrictToMaterialGroups";
  private const string NODE_APPLY_AUTOMATICALLY = "ApplyAutomatically";

  public TechnicolorPersistentZoneData(EditorColorZone edZone)
  {
    ZoneName = edZone.Name;
    DisplayName = edZone.DisplayName;
    PrimarySwatch = edZone.DefaultPrimarySwatch;
    SecondarySwatch = edZone.DefaultSecondarySwatch;
    AlwaysActive = edZone.AlwaysActive;

    RestrictToMaterialGroups = edZone.RestrictToGroupsDefault;

    if (AlwaysActive)
    {
      ActiveInEditor = true;
    }
  }

  public TechnicolorPersistentZoneData(ConfigNode node)
  {
    Load(node);
  }

  public ConfigNode Save()
  {
    var node = new ConfigNode(TechnicolorConstants.PERSISTENCE_ZONE_NODE);
    node.AddValue(NODE_NAME, ZoneName);
    node.AddValue(NODE_PRIMARY, PrimarySwatch.Name);
    node.AddValue(NODE_SECONDARY, SecondarySwatch.Name);
    node.AddValue(NODE_APPLY_AUTOMATICALLY, AutoApply);
    node.AddValue(NODE_ALWAYSACTIVE, AlwaysActive);
    node.AddValue(NODE_RESTRICT_SWATCHES, RestrictToMaterialGroups);
    return node;
  }

  public void Load(ConfigNode node)
  {
    string priSwatchName = "";
    string secSwatchName = "";
    node.TryGetValue("name", ref ZoneName);
    node.TryGetValue(NODE_PRIMARY, ref priSwatchName);
    node.TryGetValue(NODE_SECONDARY, ref secSwatchName);
    node.TryGetValue(NODE_ALWAYSACTIVE, ref AlwaysActive);
    node.TryGetValue(NODE_RESTRICT_SWATCHES, ref RestrictToMaterialGroups);
    node.TryGetValue(NODE_APPLY_AUTOMATICALLY, ref AutoApply);

    if (priSwatchName != "")
    {
      PrimarySwatch = SwatchLibrary.GetSwatch(priSwatchName);
    }

    if (secSwatchName != "")
    {
      SecondarySwatch = SwatchLibrary.GetSwatch(secSwatchName);
    }

    if (ZoneName == "main")
    {
      ActiveInEditor = true;
    }
  }
}
