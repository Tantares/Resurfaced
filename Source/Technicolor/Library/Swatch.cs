using KSP.Localization;
using UnityEngine;

namespace Technicolor;

# pragma warning disable CS0649

public class Swatch
{
  [Persistent] public readonly string Name = "default";
  [Persistent(name = "DisplayName")] private readonly string _displayName;
  public readonly string DisplayName;
  [Persistent] public readonly string Group = "internal";
  public readonly Color Color = Color.white;
  [Persistent] public readonly float Metalness = 0f;
  [Persistent] public readonly float Smoothness = 0.2f;
  [Persistent] public readonly float MetalBlend = 1f;
  [Persistent] public readonly float SmoothBlend = 1f;

  public Sprite Thumbnail;
  public Sprite ThumbnailLeft;
  public Sprite ThumbnailRight;

  public Swatch() { }

  public Swatch(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    DisplayName = Localizer.Format(_displayName ?? Name);
    Color = ParsedMultiColor.Parse(node.GetValue("Color"));
  }

  public void GenerateThumbnail()
  {
    if (Thumbnail != null) return;

    Utils.Log($"[TechnicolorSwatch] Rendering swatch thumbnails for {Name}", LogType.UI);

    var sky = TechnicolorAssets.SwatchRenderSkybox;
    sky.SetTexture(ShaderPropertyID._Tex, TechnicolorAssets.SwatchRenderTexture);
    sky.SetColor(ShaderPropertyID._Tint, new(0.3f, 0.3f, 0.3f));

    var texture = SwatchRenderUtility.RenderSwatchThumbnail(
      TechnicolorAssets.SwatchRenderMultiPrefab,
      Settings.SwatchRenderResolution,
      sky,
      this);
    ThumbnailLeft = Sprite.Create(texture,
                                  new(0.0f,
                                      0.0f,
                                      Settings.SwatchRenderResolution,
                                      texture.height),
                                  new(0.5f, 0.5f));
    ThumbnailRight = Sprite.Create(texture,
                                   new(Settings.SwatchRenderResolution,
                                       0.0f,
                                       Settings.SwatchRenderResolution,
                                       texture.height),
                                   new(0.5f, 0.5f));
    Thumbnail = Sprite.Create(texture,
                              new(Settings.SwatchRenderResolution * 2,
                                  0.0f,
                                  Settings.SwatchRenderResolution,
                                  texture.height),
                              new(0.5f, 0.5f));
  }
}
