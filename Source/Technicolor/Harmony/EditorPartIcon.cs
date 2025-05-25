using HarmonyLib;
using KSP.UI.Screens;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Steamworks;
using System.Collections.Generic;

namespace Technicolor.HarmonyPatches;

[HarmonyPatch(typeof(EditorPartIcon))]
public class PatchEditorPartIcon
{
  /// <summary>
  /// Patch the icon creation process to add a thingy
  /// </summary>
  /// <param name="__instance"></param>
  [HarmonyPostfix]
  [HarmonyPatch("Create", new Type[] { typeof(EditorPartList), typeof(AvailablePart), typeof(StoredPart), typeof(float), typeof(float), typeof(float), typeof(Callback<EditorPartIcon>), typeof(bool), typeof(bool), typeof(PartVariant), typeof(bool), typeof(bool) })]
  static void PatchCreate(EditorPartIcon __instance)
  {
    if (!__instance.inInventory && RDController.Instance == null)
    {
      ModuleTechnicolor technicolor = __instance.AvailPart.partPrefab.GetComponent<ModuleTechnicolor>();
      if (technicolor != null)
      {
        AvailablePart part = __instance.AvailPart;

        Image swatch = new GameObject("Paintable").AddComponent<Image>();
        swatch.sprite = TechnicolorAssets.GetSprite("tech-icon-brush-mini");
        swatch.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
        swatch.transform.SetParent(__instance.gameObject.transform, false);

        RectTransform rect = swatch.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.one;

        rect.offsetMin = new Vector2(-16, -16);
        rect.offsetMax = new Vector2(-2, -2);
      }
    }
  }


}
