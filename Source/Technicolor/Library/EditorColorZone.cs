using System;
using KSP.Localization;

namespace Technicolor;

#pragma warning disable CS0649

public class EditorColorZone
{
  [Persistent] public readonly string Name = "default";
  [Persistent(name = "DisplayName")] private readonly string _displayName;
  public readonly string DisplayName;
  [Persistent] public readonly bool AlwaysActive = false;
  [Persistent] public readonly bool RestrictToGroupsDefault = false;

  private const string NODE_VALID_GROUP = "ValidGroup";
  public readonly string[] ValidGroups = [];

  [Persistent] public readonly string DefaultPrimarySwatchName = SwatchLibrary.DefaultSwatch.Name;
  [Persistent] public readonly string DefaultSecondarySwatchName = SwatchLibrary.DefaultSwatch.Name;

  public EditorColorZone(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    ValidGroups = node.GetValues(NODE_VALID_GROUP);
    DisplayName = Localizer.Format(_displayName ?? Name);

    if (String.IsNullOrEmpty(Name))
      throw new ArgumentException("[EditorColorZone] Must provide a name");

    if (!SwatchLibrary.Has(DefaultPrimarySwatchName))
    {
      Utils.LogWarning($"[EditorColorZone] Swatch {DefaultPrimarySwatchName} not found");
      DefaultPrimarySwatchName = SwatchLibrary.DefaultSwatch.Name;
    }

    if (!SwatchLibrary.Has(DefaultSecondarySwatchName))
    {
      Utils.LogWarning($"[EditorColorZone] Swatch {DefaultSecondarySwatchName} not found");
      DefaultSecondarySwatchName = SwatchLibrary.DefaultSwatch.Name;
    }
  }
}
