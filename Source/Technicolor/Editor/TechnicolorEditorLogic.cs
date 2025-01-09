using UnityEngine;

namespace Technicolor;

[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class TechnicolorEditorLogic : MonoBehaviour
{
  public static TechnicolorSwatchData SwatchData;
  public static TechnicolorEditorLogic Instance;
  public static TechnicolorEditorRollover Rollover;

  public void Start()
  {
    if (SwatchData == null)
    {
      SwatchData = new();
    }

    EditorLogic.fetch.toolsUI.gameObject.AddComponent<TechnicolorEditorModes>();
    GameEvents.onVariantApplied.Add(new(OnPartVariantApplied));

    Rollover = gameObject.AddComponent<TechnicolorEditorRollover>();
  }

  public static void GetSwatchesFromPart(ModuleTechnicolor module)
  {
    Utils.Log($"[TechnicolorEditorLogic] Getting swatches from part", LogType.Editor);
    module.GetPartSwatches(ref SwatchData);
    TechnicolorUI.Instance.MaterialWindow.SetUISwatches();
  }

  public static void PaintPart(ModuleTechnicolor module)
  {
    Utils.Log($"[TechnicolorEditorLogic] Painting part", LogType.Editor);
    module.SetPartSwatches(SwatchData);
  }

  /// <summary>
  /// This function exists purely to deal with ModulePartVariants resetting materials when variants change
  /// </summary>
  /// <param name="part"></param>
  /// <param name="partVariant"></param>
  public void OnPartVariantApplied(Part part, PartVariant partVariant)
  {
    if (part != null)
    {
      var module = part.GetComponent<ModuleTechnicolor>();
      if (module != null)
      {
        //module.ApplySwatches();
        Utils.Log($"[TechnicolorEditorLogic] Painting part", LogType.Editor);
      }
    }
  }

  private void Awake()
  {
    Instance = this;
  }

  private void OnDestroy()
  {
    Instance = null;
    GameEvents.onVariantApplied.Remove(OnPartVariantApplied);
  }
}
