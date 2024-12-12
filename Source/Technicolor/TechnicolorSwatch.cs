using UnityEngine;

namespace Technicolor
{
  [System.Serializable]
  public class TechnicolorSwatch
  {
    public string Name => _name;
    public string DisplayName => _DisplayName;
    public string Group => _group;
    public Color Color => _Color;
    public float Metalness => _Metalness;
    public float Smoothness => _Smoothness;
    public float MetalBlend => _MetalBlend;
    public float SmoothBlend => _SmoothBlend;

    public Sprite Thumbnail;
    public Sprite ThumbnailLeft;
    public Sprite ThumbnailRight;

    private Texture2D texture;

    private string _name;
    private string _DisplayName;
    private string _group;
    private Color _Color;
    private float _Metalness;
    private float _Smoothness;
    private float _MetalBlend;
    private float _SmoothBlend;

    private const string NODE_NAME = "Name";
    private const string NODE_GROUP = "Group";
    private const string NODE_DISPLAYNAME = "DisplayName";
    private const string NODE_COLOR = "Color";
    private const string NODE_METALNESS = "Metalness";
    private const string NODE_SMOOTHNESS = "Smoothness";
    private const string NODE_METALBLEND = "MetalBlend";
    private const string NODE_SMOOTHBLEND = "SmoothBlend";

    public TechnicolorSwatch()
    {
      _name = "default";
      _group = "internal";
      _DisplayName = "Default";
      _Color = Color.white;
      _Metalness = 0f;
      _Smoothness = 0.2f;
      _SmoothBlend = 1.0f;
      _MetalBlend = 1.0f;
    }
    public TechnicolorSwatch(ConfigNode node)
    {
      Load(node);
    }

    public void Load(ConfigNode node)
    {
      node.TryGetValue(NODE_NAME, ref _name);
      node.TryGetValue(NODE_GROUP, ref _group);
      node.TryGetValue(NODE_DISPLAYNAME, ref _DisplayName);
      node.TryGetValue(NODE_COLOR, ref _Color);
      node.TryGetValue(NODE_METALNESS, ref _Metalness);
      node.TryGetValue(NODE_SMOOTHNESS, ref _Smoothness);
      node.TryGetValue(NODE_METALBLEND, ref _MetalBlend);
      node.TryGetValue(NODE_SMOOTHBLEND, ref _SmoothBlend);
    }
    public void Save(ConfigNode parent)
    {
      ConfigNode node = new ConfigNode();
      parent.AddNode(node);
    }
    public void GenerateThumbnail()
    {

      if (!Thumbnail)
      {
        Utils.Log($"[TechnicolorSwatch] Rendering swatch thumbnails for {_name}", LogType.UI);

        Material sky = TechnicolorAssets.SwatchRenderSkybox;
        sky.SetTexture("_Tex", TechnicolorAssets.SwatchRenderTexture);

        UnityEngine.Object.Destroy(texture);

        texture = SwatchRenderUtility.RenderSwatchThumbnail(TechnicolorAssets.SwatchRenderMultiPrefab, Settings.SwatchRenderResolution, sky, this);
        ThumbnailLeft = Sprite.Create(texture, new Rect(0.0f, 0.0f, Settings.SwatchRenderResolution, texture.height), new Vector2(0.5f, 0.5f));
        ThumbnailRight = Sprite.Create(texture, new Rect(Settings.SwatchRenderResolution, 0.0f, Settings.SwatchRenderResolution, texture.height), new Vector2(0.5f, 0.5f));
        Thumbnail = Sprite.Create(texture, new Rect(Settings.SwatchRenderResolution * 2, 0.0f, Settings.SwatchRenderResolution, texture.height), new Vector2(0.5f, 0.5f));
        return;
      }
      Utils.Log($"[TechnicolorSwatch] Skipping render for {_name}", LogType.UI);
    }
  }
}
