namespace Technicolor;

[KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR)]
public class TechnicolorPersistence : ScenarioModule
{
  public static TechnicolorPersistence Instance { get; private set; }

  public override void OnAwake()
  {
    Instance = this;
    base.OnAwake();
  }

  public override void OnLoad(ConfigNode node)
  {
    Utils.Log("[TechnicolorPersistence]: Started Loading", LogType.Loading);
    base.OnLoad(node);
    var swatchSpec = node.GetNode(Constants.PERSISTENCE_NODE);
    if (swatchSpec != null && TechnicolorEditorLogic.EditorData != null)
    {
      TechnicolorEditorLogic.EditorData.Load(swatchSpec);
    }

    Utils.Log("[TechnicolorPersistence]: Done Loading", LogType.Loading);
  }

  public override void OnSave(ConfigNode node)
  {
    Utils.Log("[TechnicolorPersistence]: Started Saving", LogType.Loading);
    base.OnSave(node);
    ConfigNode swatchNode = new(Constants.PERSISTENCE_NODE);
    TechnicolorEditorLogic.EditorData.Save(swatchNode);
    node.AddNode(swatchNode);

    Utils.Log("[TechnicolorPersistence]: Finished Saving", LogType.Loading);
  }
}
