using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technicolor;

public enum LogType
{
  UI,
  Settings,
  Loading,
  Editor,
  Any
}

public static class Utils
{
  public static string ModName = "Technicolor";

  public static bool IsLoadingPrefab() => !PartLoader.Instance.IsReady();

  /// <summary>
  ///   Log a message with the mod name tag prefixed
  /// </summary>
  /// <param name="str">message string </param>
  public static void Log(string str)
  {
    Log(str, LogType.Any);
  }

  /// <summary>
  /// Is logging enabled?
  /// </summary>
  /// <param name="logType">Logging Type</param>
  /// <returns>True if logging is enabled</returns>
  public static bool IsLogging(LogType logType = LogType.Any)
  {
    return logType == LogType.Any
        || logType == LogType.Settings && Settings.DebugSettings
        || logType == LogType.UI && Settings.DebugUI
        || logType == LogType.Loading && Settings.DebugLoading
        || logType == LogType.Editor && Settings.DebugEditor;
  }

  /// <summary>
  ///   Log a message with the mod name tag prefixed
  /// </summary>
  /// <param name="str">message string </param>
  public static void Log(string str, LogType logType)
  {
    if (IsLogging(logType))
      Debug.Log($"[{ModName}]{str}");
  }

  /// <summary>
  ///   Log an error with the mod name tag prefixed
  /// </summary>
  /// <param name="str">Error string </param>
  public static void LogError(string str)
  {
    Debug.LogError(String.Format("[{0}]{1}", ModName, str));
  }

  /// <summary>
  ///   Log a warning with the mod name tag prefixed
  /// </summary>
  /// <param name="str">warning string </param>
  public static void LogWarning(string str)
  {
    Debug.LogWarning(String.Format("[{0}]{1}", ModName, str));
  }
}

public static class TransformDeepChildExtension
{
  public static Transform FindDeepChild(this Transform aParent, string aName)
  {
    var queue = new Queue<Transform>();
    queue.Enqueue(aParent);
    while (queue.Count > 0)
    {
      var c = queue.Dequeue();
      if (c.name == aName)
        return c;
      foreach (Transform t in c)
        queue.Enqueue(t);
    }

    return null;
  }
}

/// <summary>
/// Stores a Color as a Hue/Saturation/Value model with components 0-1
/// </summary>
[Serializable]
public class ColorHSV
{
  public float h = 0.5f;
  public float s = 0.5f;
  public float v = 0.5f;
  public float a = 1f;
  public ColorHSV() { }

  /// <summary>
  /// New HSV color from components
  /// </summary>
  /// <param name="hue"></param>
  /// <param name="sat"></param>
  /// <param name="value"></param>
  public ColorHSV(float hue, float sat, float value)
  {
    h = hue;
    s = sat;
    v = value;
    a = 1f;
  }

  /// <summary>
  /// New HSV color from components
  /// </summary>
  /// <param name="hue"></param>
  /// <param name="sat"></param>
  /// <param name="value"></param>
  /// <param name="alpha"></param>
  public ColorHSV(float hue, float sat, float value, float alpha)
  {
    h = hue;
    s = sat;
    v = value;
    a = alpha;
  }

  /// <summary>
  /// New HSV color from RGBA
  /// </summary>
  /// <param name="colorRGB"></param>
  public ColorHSV(Color colorRGB)
  {
    Color.RGBToHSV(colorRGB, out h, out s, out v);
    a = colorRGB.a;
  }

  /// <summary>
  /// Tests color equality
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(ColorHSV other)
  {
    return h == other.h && s == other.s && v == other.v && a == other.a;
  }

  public override string ToString()
  {
    return $"ColorHSV({h}, {s}, {v}, {a})";
  }

  /// <summary>
  /// Returns this color as RGBA
  /// </summary>
  /// <returns></returns>
  public Color ToRGB()
  {
    var newC = Color.HSVToRGB(h, s, v);
    newC = new(newC.r, newC.g, newC.b, a);
    return newC;
  }
}
