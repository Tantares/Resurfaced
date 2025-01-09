using KSP.Localization;
using UnityEngine;

namespace Technicolor;

public class Swatch
{
  [Persistent] public string Name = "default";
  [Persistent(name = "DisplayName")] private string _displayName = "Default";
  public string DisplayName = "Default";
  [Persistent] public string Group = "internal";
  [Persistent] public Color Color = Color.white;
  [Persistent] public float Metalness = 0f;
  [Persistent] public float Smoothness = 0.2f;
  [Persistent] public float MetalBlend = 1f;
  [Persistent] public float SmoothBlend = 1f;

  public Sprite Thumbnail;
  public Sprite ThumbnailLeft;
  public Sprite ThumbnailRight;

  public Swatch() { }

  public Swatch(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
    DisplayName = Localizer.Format(_displayName);
  }

  public void GenerateThumbnail()
  {
    if (Thumbnail != null)
    {
      return;
    }

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
