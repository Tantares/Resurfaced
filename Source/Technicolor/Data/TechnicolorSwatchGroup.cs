using KSP.Localization;

namespace Technicolor;

public class TechnicolorSwatchGroup
{
  [Persistent(name = "name")] public string Name = "default";
  [Persistent] public string DisplayName = "Default";

  public TechnicolorSwatchGroup() { }

  public TechnicolorSwatchGroup(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    DisplayName = Localizer.Format(DisplayName);
  }
}
