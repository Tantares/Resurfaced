using KSP.UI;
using KSP.UI.TooltipTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor;

public static class Tooltips
{
  public static TooltipController_Text AddTooltip(
    GameObject targetObject,
    Tooltip_Text prefab,
    string tooltipText)
  {
    if (!targetObject.GetComponent<Selectable>())
    {
      var sel = targetObject.AddComponent<Selectable>();
      sel.navigation = new()
      {
        mode = Navigation.Mode.None
      };
    }

    var tooltip = targetObject.AddComponent<TooltipController_Text>();
    tooltip.prefab = prefab;
    tooltip.RequireInteractable = false;
    tooltip.textString = tooltipText;
    return tooltip;
  }

  public static Tooltip_Text FindTextTooltipPrefab()
  {
    if (HighLogic.LoadedSceneIsEditor)
    {
      var sorterBase = GameObject.FindObjectOfType<UIListSorter>();
      var sortByNameButton = sorterBase.gameObject.GetChild("StateButtonName");
      return sortByNameButton.GetComponent<TooltipController_Text>().prefab;
    }
    else
    {
      return GameObject.FindObjectOfType<TooltipController_Text>().prefab;
    }
  }
}
