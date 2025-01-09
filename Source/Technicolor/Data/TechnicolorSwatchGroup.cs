using KSP.Localization;

namespace Technicolor;

[System.Serializable]
public class TechnicolorSwatchGroup
{
  public string Name => _name;
  public string DisplayName => _displayName;

  private string _name;
  private string _displayName;

  private const string NODE_NAME = "name";
  private const string NODE_DISPLAYNAME = "DisplayName";

  public TechnicolorSwatchGroup()
  {
    _name = "default";
    _displayName = "Default";
  }

  public TechnicolorSwatchGroup(ConfigNode node)
  {
    Load(node);
  }

  public void Load(ConfigNode node)
  {
    node.TryGetValue(NODE_NAME, ref _name);
    node.TryGetValue(NODE_DISPLAYNAME, ref _displayName);

    _displayName = Localizer.Format(_displayName);
  }
}
