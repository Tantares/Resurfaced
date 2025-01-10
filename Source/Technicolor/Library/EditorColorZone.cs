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

  [Persistent] public string DefaultPrimarySwatch = SwatchLibrary.DefaultSwatch.Name;
  [Persistent] public string DefaultSecondarySwatch = SwatchLibrary.DefaultSwatch.Name;

  public EditorColorZone() { }

  public EditorColorZone(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    ValidGroups = node.GetValues(NODE_VALID_GROUP);
    DisplayName = Localizer.Format(_displayName);
  }
}
