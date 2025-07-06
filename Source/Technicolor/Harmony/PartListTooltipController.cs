using HarmonyLib;
using KSP.UI.Screens;
using System;
using KSP.UI.Screens.Editor;

namespace Technicolor.HarmonyPatches;

[HarmonyPatch(typeof(PartListTooltipController))]
public class PatchPartListTooltipController
{
  /// <summary>
  /// Patch the icon creation process to add a thingy
  /// </summary> PartListTooltip tooltip, EditorPartIcon partIcon
  /// <param name="__instance"></param>
  [HarmonyPostfix]
  [HarmonyPatch("CreateTooltip", new Type[] { typeof(PartListTooltip), typeof(EditorPartIcon) })]
  static void PatchCreateTooltip(PartListTooltipController __instance, PartListTooltip tooltip, EditorPartIcon partIcon)
  {
    if (!__instance.editorPartIcon.inInventory && RDController.Instance == null)
    {
      ModuleTechnicolor technicolor = __instance.partInfo.partPrefab.GetComponent<ModuleTechnicolor>();
      if (technicolor != null)
      {
        MaterialUtils.ApplyMaterials(technicolor, tooltip.partIcon.icon);        
      }
    }
  }
  /// <summary>
  /// Patch the icon creation process to add a thingy
  /// </summary> PartListTooltip tooltip, EditorPartIcon partIcon
  /// <param name="__instance"></param>
  [HarmonyPostfix]
  [HarmonyPatch("onVariantToggle", new Type[] { typeof(PartVariant)})]
  static void PatchOnVariantToggle(PartListTooltipController __instance)
  {
    if (!__instance.editorPartIcon.inInventory && RDController.Instance == null)
    {
      ModuleTechnicolor technicolor = __instance.partInfo.partPrefab.GetComponent<ModuleTechnicolor>();
      if (technicolor != null)
      {
        MaterialUtils.ApplyMaterials(technicolor, __instance.tooltipInstance.partIcon.icon);
      }
    }
  }
}
