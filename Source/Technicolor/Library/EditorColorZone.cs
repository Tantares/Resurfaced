using KSP.Localization;

namespace Technicolor;

public class EditorColorZone
{
  [Persistent] public readonly string Name = "Default";
  [Persistent(name = "DisplayName")] private readonly string _displayName = "Default";
  public readonly string DisplayName = "Default";
  [Persistent] public readonly bool AlwaysActive = false;
  [Persistent] public readonly bool RestrictToGroupsDefault = false;

  private const string NODE_VALID_GROUP = "ValidGroup";
  public readonly string[] ValidGroups = [];

  [Persistent] public readonly string DefaultPrimarySwatchName = SwatchLibrary.DefaultSwatch.Name;
  [Persistent] public readonly string DefaultSecondarySwatchName = SwatchLibrary.DefaultSwatch.Name;

  public EditorColorZone() { }

  public EditorColorZone(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    ValidGroups = node.GetValues(NODE_VALID_GROUP);
    DisplayName = Localizer.Format(_displayName);
  }
}
