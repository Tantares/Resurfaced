﻿
namespace Technicolor
{
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
      ConfigNode swatchData = node.GetNode(TechnicolorConstants.SWATCH_PERSISTENCE_NODE);
      if (swatchData != null && TechnicolorEditorLogic.SwatchData != null)
      {
        TechnicolorEditorLogic.SwatchData.Load(swatchData);
      }
      Utils.Log("[TechnicolorPersistence]: Done Loading", LogType.Loading);

    }

    public override void OnSave(ConfigNode node)
    {
      Utils.Log("[TechnicolorPersistence]: Started Saving", LogType.Loading);
      base.OnSave(node);
      ConfigNode swatchNode = new(TechnicolorConstants.SWATCH_PERSISTENCE_NODE);
      TechnicolorEditorLogic.SwatchData.Save(swatchNode);
      node.AddNode(swatchNode);
        
      Utils.Log("[TechnicolorPersistence]: Finished Saving", LogType.Loading);
    }

    
  }
}