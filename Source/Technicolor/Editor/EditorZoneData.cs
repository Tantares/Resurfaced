namespace Technicolor;

public class EditorZoneData
{
  [Persistent] public readonly string Name;

  public EditorColorZone EditorZone => ZoneLibrary.EditorColorZones[Name];

  public string DisplayName => EditorZone.DisplayName;
  public bool AlwaysActive => EditorZone.AlwaysActive;

  [Persistent(name = "PrimarySwatch")] private string _primarySwatchName;
  [Persistent(name = "SecondarySwatch")] private string _secondarySwatchName;

  public Swatch PrimarySwatch
  {
    get => SwatchLibrary.GetSwatch(_primarySwatchName);
    set => _primarySwatchName = value.Name;
  }

  public Swatch SecondarySwatch
  {
    get => SwatchLibrary.GetSwatch(_secondarySwatchName);
    set => _secondarySwatchName = value.Name;
  }

  [Persistent] public bool RestrictToMaterialGroups = true;
  [Persistent] public bool AutoApply = false;
  [Persistent] public bool ActiveInEditor = false;

  public EditorZoneData(EditorColorZone edZone)
  {
    Name = edZone.Name;
    _primarySwatchName = edZone.DefaultPrimarySwatch;
    _secondarySwatchName = edZone.DefaultSecondarySwatch;
    RestrictToMaterialGroups = edZone.RestrictToGroupsDefault;
    ActiveInEditor = AlwaysActive;
  }

  public EditorZoneData(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
  }

  public ConfigNode Save()
  {
    var node = new ConfigNode(Constants.PERSISTENCE_ZONE_NODE);
    ConfigNode.CreateConfigFromObject(this, node);
    return node;
  }
}
