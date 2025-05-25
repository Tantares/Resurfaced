namespace Technicolor;

public class EditorZoneData : ZoneDataBase
{
  public bool AlwaysActive => EditorZone.AlwaysActive;

  [Persistent] public bool RestrictToMaterialGroups = true;
  [Persistent] public bool AutoApply = false;
  [Persistent] public bool ActiveInEditor = false;

  public EditorZoneData(EditorColorZone edZone)
  {
    Name = edZone.Name;
    _primarySwatchName = edZone.DefaultPrimarySwatchName;
    _secondarySwatchName = edZone.DefaultSecondarySwatchName;
    RestrictToMaterialGroups = edZone.RestrictToGroupsDefault;
    ActiveInEditor = AlwaysActive;
  }

  public EditorZoneData(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
  }

  public void Save(ConfigNode node)
  {
    ConfigNode.CreateConfigFromObject(this, node);
  }
}
