using UnityEngine;

namespace Technicolor
{
  [KSPAddon(KSPAddon.Startup.EditorAny, false)]
  public class TechnicolorEditorLogic : MonoBehaviour
  {

    public static TechnicolorSwatchData SwatchData;
    public static TechnicolorEditorLogic Instance;


    public void Start()
    {
      if (SwatchData == null)
      {
        SwatchData = new();
      }
      EditorLogic.fetch.toolsUI.gameObject.AddComponent<TechnicolorEditorModes>();
    }

    public static void SetSwatchesSampled()
    {
      TechnicolorUI.Instance.MaterialWindow.ResetEditorUISwatches();
    }

    void Awake()
    {
      Instance = this;
    }

    void OnDestroy()
    {
      Instance = null;
    }
  }  
}
