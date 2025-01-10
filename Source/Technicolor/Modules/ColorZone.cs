using System.Collections.Generic;
using System.Linq;

namespace Technicolor;

public class ColorZone
{
  [Persistent(name = "name")] public readonly string Name = "main";

  [Persistent(name = "swatchPrimary")]
  public readonly string DefaultPrimarySwatchName = SwatchLibrary.DefaultSwatch.Name;

  [Persistent(name = "swatchSecondary")]
  public readonly string DefaultSecondarySwatchName = SwatchLibrary.DefaultSwatch.Name;

  public const string KEY_TRANSFORM = "transform";
  public readonly HashSet<string> Transforms = [];

  public ColorZone() { }

  public ColorZone(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    Transforms = node.GetValues(KEY_TRANSFORM).ToHashSet();
  }
}
