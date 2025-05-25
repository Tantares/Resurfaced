using System;
using System.Collections.Generic;
using System.Linq;

namespace Technicolor;

/// <summary>
/// Guarantees: `Name` exists and is valid. The default swatch names exist and are valid.
/// </summary>
public class ColorZone
{
  [Persistent(name = "name")] public readonly string Name;
  [Persistent(name = "swatchPrimary")] public readonly string DefaultPrimarySwatchName;
  [Persistent(name = "swatchSecondary")] public readonly string DefaultSecondarySwatchName;
  public const string KEY_TRANSFORM = "transform";
  public readonly HashSet<string> Transforms = [];

  public ColorZone()
  {
    Name = Settings.DefaultZoneName;
    DefaultPrimarySwatchName = ZoneLibrary.EditorColorZones[Name].DefaultPrimarySwatchName;
    DefaultSecondarySwatchName = ZoneLibrary.EditorColorZones[Name].DefaultSecondarySwatchName;
  }

  public ColorZone(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    Transforms = node.GetValues(KEY_TRANSFORM).ToHashSet();

    if (String.IsNullOrEmpty(Name) || !ZoneLibrary.EditorColorZones.ContainsKey(Name))
    {
      throw new ArgumentException($"[ColorZone] Zone {Name} does not exist");
    }

    if (!SwatchLibrary.Has(DefaultPrimarySwatchName))
    {
      Utils.LogWarning($"[ColorZone] Swatch {DefaultPrimarySwatchName} not found");
      DefaultPrimarySwatchName = ZoneLibrary.EditorColorZones[Name].DefaultPrimarySwatchName;
    }

    if (!SwatchLibrary.Has(DefaultSecondarySwatchName))
    {
      Utils.LogWarning($"[ColorZone] Swatch {DefaultSecondarySwatchName} not found");
      DefaultSecondarySwatchName = ZoneLibrary.EditorColorZones[Name].DefaultSecondarySwatchName;
    }
  }
}
