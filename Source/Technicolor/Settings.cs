using UnityEngine;

namespace Technicolor;

/// <summary>
///   Static class to hold settings and configuration
/// </summary>
public static class Settings
{
  /// Settings go here
  public static bool DebugSettings = true;

  public static bool DebugUI = true;
  public static bool DebugLoading = true;
  public static bool DebugEditor = true;

  public static KeyCode PaintModeKey = KeyCode.Alpha6;
  public static KeyCode SampleModeKey = KeyCode.Alpha7;
  public static KeyCode FillModeKey = KeyCode.Alpha8;
  public static KeyCode TogglePaletteKey = KeyCode.Alpha9;

  public static int SwatchRenderResolution = 64;

  public static string DefaultPrimarySwatch = "porkjetWhite";
  public static string DefaultSecondarySwatch = "porkjetBlack";

  private static bool _loadedOnce = false;

  /// <summary>
  ///   Load data from configuration
  /// </summary>
  public static void Load()
  {
    if (_loadedOnce)
      return;

    _loadedOnce = true;
    var settingsNode =
      GameDatabase.Instance.GetConfigNode(Constants.SETTINGS_CONFIG_NODE);

    Utils.Log("[Settings]: Started loading", LogType.Settings);
    if (settingsNode != null)
    {
      Utils.Log("[Settings]: Using specified settings", LogType.Settings);
      // Setting parsing goes here

      settingsNode.TryGetValue("DebugSettings", ref DebugSettings);
      settingsNode.TryGetValue("DebugLoading", ref DebugLoading);
      settingsNode.TryGetValue("DebugUI", ref DebugUI);
      settingsNode.TryGetValue("DebugEditor", ref DebugEditor);

      settingsNode.TryGetValue("DefaultPrimarySwatch", ref DefaultPrimarySwatch);
      settingsNode.TryGetValue("DefaultSecondarySwatch", ref DefaultSecondarySwatch);

      settingsNode.TryGetEnum<KeyCode>("PaintModeKey", ref PaintModeKey, KeyCode.Alpha5);
      settingsNode.TryGetEnum<KeyCode>("SampleModeKey", ref SampleModeKey, KeyCode.Alpha5);
      settingsNode.TryGetEnum<KeyCode>("FillModeKey", ref FillModeKey, KeyCode.Alpha5);
      settingsNode.TryGetEnum<KeyCode>("TogglePaletteKey", ref TogglePaletteKey, KeyCode.Alpha5);

      settingsNode.TryGetValue("SwatchRenderResolution", ref SwatchRenderResolution);
    }
    else
    {
      Utils.Log("[Settings]: Couldn't find settings file, using defaults", LogType.Settings);
    }

    Utils.Log("[Settings]: Finished loading", LogType.Settings);
  }
}
