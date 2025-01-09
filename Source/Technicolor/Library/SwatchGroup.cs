using KSP.Localization;

namespace Technicolor;

public class SwatchGroup
{
  [Persistent(name = "name")] public string Name = "default";
  [Persistent] public string DisplayName = "Default";

  public SwatchGroup() { }

  public SwatchGroup(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    DisplayName = Localizer.Format(DisplayName);
  }
}
