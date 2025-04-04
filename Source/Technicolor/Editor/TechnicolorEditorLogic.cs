namespace Technicolor;

[KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR)]
public class TechnicolorEditorLogic : ScenarioModule
{
  public static TechnicolorEditorLogic Instance;

  public static EditorData EditorData;
  public EditorRollover Rollover;

  public override void OnAwake()
  {
    Instance = this;
    EditorData ??= new();
  }

  public override void OnLoad(ConfigNode node)
  {
    Utils.Log("[TechnicolorEditorLogic]: Started loading data", LogType.Loading);
    base.OnLoad(node);
    var persistedData = node.GetNode(Constants.PERSISTENCE_NODE);
    if (persistedData != null)
    {
      EditorData.Load(persistedData);
    }

    Utils.Log("[TechnicolorEditorLogic]: Done loading data", LogType.Loading);
  }

  public void Start()
  {
    EditorLogic.fetch.toolsUI.gameObject.AddComponent<TechnicolorEditorModes>();

    Rollover = gameObject.AddComponent<EditorRollover>();
  }

  public override void OnSave(ConfigNode node)
  {
    Utils.Log("[TechnicolorEditorLogic]: Started saving data", LogType.Loading);
    base.OnSave(node);
    ConfigNode swatchNode = new(Constants.PERSISTENCE_NODE);
    EditorData.Save(swatchNode);
    node.AddNode(swatchNode);

    Utils.Log("[TechnicolorEditorLogic]: Finished saving data", LogType.Loading);
  }

  public void OnDestroy()
  {
    Instance = null;
    Rollover.OnDestroy();

    Destroy(Rollover);
  }

  public static void GetSwatchesFromPart(ModuleTechnicolor module)
  {
    Utils.Log($"[TechnicolorEditorLogic] Getting swatches from part", LogType.Editor);
    module.GetPartSwatches(ref EditorData);
    TechnicolorUI.Instance.MaterialWindow.SetUISwatches();
  }

  public static void PaintPart(ModuleTechnicolor module)
  {
    Utils.Log($"[TechnicolorEditorLogic] Painting part", LogType.Editor);
    module.SetAllSwatches(EditorData);
  }

}
