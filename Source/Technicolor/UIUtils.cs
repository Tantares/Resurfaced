using System;
using KSP.UI;
using KSP.UI.TooltipTypes;
using UnityEngine;

namespace Technicolor;

/// <summary>
/// Get a reference in a child of a type
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="name"></param>
/// <param name="parent"></param>
/// <returns></returns>
public static class UIUtils
{
  public static T FindChildOfType<T>(string name, Transform parent)
  {
    var result = default(T);
    try
    {
      result = parent.FindDeepChild(name).GetComponent<T>();
    }
    catch (NullReferenceException e)
    {
      Debug.LogError($"Couldn't find {name} in children of {parent.name}");
    }

    return result;
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
