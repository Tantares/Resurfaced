using KSP.Localization;
namespace Technicolor
{

  [System.Serializable]
  public class EditorColorZone
  {
    public string Name => _name;
    public string DisplayName => _displayName;
    public bool AlwaysActive => _alwaysActive;
    public bool RestrictToGroupsDefault => _restrictToGroupsDefault;
    public string[] ValidGroups => _validGroups;

    public TechnicolorSwatch DefaultPrimarySwatch => _defaultPrimarySwatch;
    public TechnicolorSwatch DefaultSecondarySwatch => _defaultSecondarySwatch;

    private string _name;
    private string _displayName;
    private bool _alwaysActive = false;
    private bool _restrictToGroupsDefault = false;
    private string[] _validGroups;
    private string _defaultPrimarySwatchName = "";
    private string _defaultSecondarySwatchName = "";
    private TechnicolorSwatch _defaultPrimarySwatch;
    private TechnicolorSwatch _defaultSecondarySwatch;

    private const string NODE_NAME = "Name";
    private const string NODE_RESTRICT_TO_GROUP = "RestrictToGroupsDefault";
    private const string NODE_DISPLAYNAME = "DisplayName";
    private const string NODE_ALWAYSACTIVE = "AlwaysActive";
    private const string NODE_VALID_GROUP = "ValidGroup";
    private const string NODE_DEFAULT_PRIMARY = "DefaultPrimarySwatch";
    private const string NODE_DEFAULT_SECONDARY = "DefaultSecondarySwatch";

    public EditorColorZone()
    {

      _name = "default";
      _displayName = "Default";
      _restrictToGroupsDefault = false;
      _validGroups = new string[0];
    }
    public EditorColorZone(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue(NODE_NAME, ref _name);
      node.TryGetValue(NODE_RESTRICT_TO_GROUP, ref _restrictToGroupsDefault);
      node.TryGetValue(NODE_DISPLAYNAME, ref _displayName);
      node.TryGetValue(NODE_ALWAYSACTIVE, ref _alwaysActive);
      _validGroups = node.GetValues(NODE_VALID_GROUP);
      node.TryGetValue(NODE_DEFAULT_PRIMARY, ref _defaultPrimarySwatchName);
      node.TryGetValue(NODE_DEFAULT_SECONDARY, ref _defaultSecondarySwatchName);

      _defaultPrimarySwatch = TechnicolorData.SwatchLibrary.GetSwatch(_defaultPrimarySwatchName);
      _defaultSecondarySwatch = TechnicolorData.SwatchLibrary.GetSwatch(_defaultSecondarySwatchName);

      _displayName = Localizer.Format(_displayName);
    }

  }
}
