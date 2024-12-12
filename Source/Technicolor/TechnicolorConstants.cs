using UnityEngine;

namespace Technicolor
{
  public static class TechnicolorConstants
  {
    public static string SETTINGS_CONFIG_NODE = "Technicolor/TechnicolorSettings/TECHNICOLOR_SETTINGS";

    public static string SWATCH_LIBRARY_CONFIG_NODE = "TECHNICOLOR_SWATCHES";
    public static string SWATCH_CONFIG_NODE = "PRESET_SWATCH";
    public static string SWATCH_PERSISTENCE_NODE = "EDITORSWATCHCONFIG";

    public static string MODULE_COLOR_NODE = "COLORZONE";

    public static string TEAMCOLOR_SHADER_NAME = "Resurfaced/Standard (TC)";
  }
  public static class ShaderPropertyID
  {
    public static readonly int _TC1Color = Shader.PropertyToID(nameof(_TC1Color));
    public static readonly int _TC1Metalness = Shader.PropertyToID(nameof(_TC1Metalness));
    public static readonly int _TC1Smoothness = Shader.PropertyToID(nameof(_TC1Smoothness));
    public static readonly int _TC1SmoothBlend = Shader.PropertyToID(nameof(_TC1SmoothBlend));
    public static readonly int _TC1MetalBlend = Shader.PropertyToID(nameof(_TC1MetalBlend));

    public static readonly int _TC2Color = Shader.PropertyToID(nameof(_TC2Color));
    public static readonly int _TC2Metalness = Shader.PropertyToID(nameof(_TC2Metalness));
    public static readonly int _TC2Smoothness = Shader.PropertyToID(nameof(_TC2Smoothness));
    public static readonly int _TC2SmoothBlend = Shader.PropertyToID(nameof(_TC2SmoothBlend));
    public static readonly int _TC2MetalBlend = Shader.PropertyToID(nameof(_TC2MetalBlend));
  }
}