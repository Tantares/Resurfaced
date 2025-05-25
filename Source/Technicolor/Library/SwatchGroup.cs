using KSP.Localization;

namespace Technicolor;

#pragma warning disable CS0649

public class SwatchGroup
{
  [Persistent(name = "name")] public readonly string Name = "default";
  [Persistent(name = "DisplayName")] private readonly string _displayName;
  public readonly string DisplayName;

  public SwatchGroup(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    DisplayName = Localizer.Format(_displayName ?? Name);
  }
}
