namespace Technicolor;

[KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR)]
public class TechnicolorEditorLogic : ScenarioModule
{
  public static TechnicolorEditorLogic Instance;

  public static EditorData EditorData;
  public TechnicolorEditorRollover Rollover;

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
    GameEvents.onVariantApplied.Add(OnPartVariantApplied);

    Rollover = gameObject.AddComponent<TechnicolorEditorRollover>();
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
    GameEvents.onVariantApplied.Remove(OnPartVariantApplied);
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
    module.SetPartSwatches(EditorData);
  }

  /// <summary>
  /// This function exists purely to deal with ModulePartVariants resetting materials when variants change
  /// </summary>
  /// <param name="part"></param>
  /// <param name="partVariant"></param>
  public void OnPartVariantApplied(Part part, PartVariant partVariant)
  {
    if (part == null) return;
    var module = part.GetComponent<ModuleTechnicolor>();
    if (module == null) return;
    //module.ApplySwatches();
    Utils.Log($"[TechnicolorEditorLogic] Painting part", LogType.Editor);
  }
}
