using KSP.Localization;

namespace Technicolor;

public class EditorColorZone
{
  [Persistent] public string Name = "Default";
  [Persistent(name = "DisplayName")] private string _displayName = "Default";
  public string DisplayName = "Default";
  [Persistent] public bool AlwaysActive = false;
  [Persistent] public bool RestrictToGroupsDefault = false;

  private const string NODE_VALID_GROUP = "ValidGroup";
  public string[] ValidGroups = [];

  [Persistent(name = "DefaultPrimarySwatch")]
  private string _defaultPrimarySwatchName = SwatchLibrary.DefaultSwatch.Name;

  [Persistent(name = "DefaultSecondarySwatch")]
  private string _defaultSecondarySwatchName = SwatchLibrary.DefaultSwatch.Name;

  public TechnicolorSwatch DefaultPrimarySwatch =>
    SwatchLibrary.GetSwatch(_defaultPrimarySwatchName);

  public TechnicolorSwatch DefaultSecondarySwatch =>
    SwatchLibrary.GetSwatch(_defaultSecondarySwatchName);

  public EditorColorZone() { }

  public EditorColorZone(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    ValidGroups = node.GetValues(NODE_VALID_GROUP);
    DisplayName = Localizer.Format(_displayName);
  }
}
