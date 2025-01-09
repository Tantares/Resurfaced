using System.Collections.Generic;
using System.IO;
using UniLinq;
using UnityEngine;

namespace Technicolor;

/// <summary>
/// Loads and holds references to Asset Bundled things
/// </summary>
[KSPAddon(KSPAddon.Startup.Instantly, true)]
public class TechnicolorAssets : MonoBehaviour
{
  public static GameObject SwatchWindowPrefab { get; private set; }
  public static GameObject SwatchWidgetPrefab { get; private set; }
  public static GameObject SwatchLibraryGroupPrefab { get; private set; }
  public static GameObject SwatchLibraryButtonPrefab { get; private set; }

  public static GameObject EditorRolloverPrefab { get; private set; }
  public static GameObject SwatchRenderMultiPrefab { get; private set; }
  public static GameObject SwatchRenderSpherePrefab { get; private set; }
  public static Material SwatchRenderSkybox { get; private set; }
  public static Texture2D SwatchRenderTexture { get; private set; }

  public static Dictionary<string, Sprite> Sprites { get; private set; }

  public static Sprite GetSprite(string key)
  {
    if (Sprites.ContainsKey(key))
    {
      return Sprites[key];
    }

    Debug.LogWarning($"[Assets] Could not find sprite {key}");
    return Sprites.First().Value;
  }

  internal static string ASSET_PATH = "GameData/Technicolor/UI/technicolor.dat";
  internal static string SPRITE_ATLAS_NAME = "technicolor-sprites-1";

  private void Awake()
  {
    DontDestroyOnLoad(gameObject);
    Utils.Log("[TechnicolorAssets]: Loading Assets", LogType.Loading);
    var prefabs = AssetBundle.LoadFromFile(Path.Combine(KSPUtil.ApplicationRootPath, ASSET_PATH));

    /// Get the Prefabs
    SwatchWindowPrefab = prefabs.LoadAsset("SwatchWindow2") as GameObject;

    SwatchWidgetPrefab = prefabs.LoadAsset("ZoneSwatchWidget") as GameObject;
    SwatchWidgetPrefab.AddComponent<UISwatchZoneWidget>().AssignReferences();
    SwatchWidgetPrefab.transform.SetParent(transform);

    SwatchLibraryGroupPrefab = prefabs.LoadAsset("SwatchGroup") as GameObject;
    SwatchLibraryGroupPrefab.AddComponent<UILibrarySwatchGroup>().AssignReferences();
    SwatchLibraryGroupPrefab.transform.SetParent(transform);

    SwatchLibraryButtonPrefab = prefabs.LoadAsset("SwatchButton") as GameObject;
    SwatchLibraryButtonPrefab.AddComponent<UILibrarySwatch>().AssignReferences();
    SwatchLibraryButtonPrefab.transform.SetParent(transform);

    EditorRolloverPrefab = prefabs.LoadAsset("ZoneRolloverWidget") as GameObject;
    EditorRolloverPrefab.AddComponent<UIEditorRolloverPanel>().AssignReferences();
    EditorRolloverPrefab.transform.SetParent(transform);

    Utils.Log("[TechnicolorAssets]: Loaded UI Prefabs", LogType.Loading);

    SwatchRenderMultiPrefab = prefabs.LoadAsset("technicolor-swatch-multi") as GameObject;
    SwatchRenderSpherePrefab = prefabs.LoadAsset("technicolor-swatch-sphere") as GameObject;
    SwatchRenderSkybox = prefabs.LoadAsset("technicolor-swatch-skybox") as Material;
    SwatchRenderTexture = prefabs.LoadAsset("VAB_lr") as Texture2D;

    /// Get the Sprite Atlas
    var spriteSheet = prefabs.LoadAssetWithSubAssets<Sprite>(SPRITE_ATLAS_NAME);
    Sprites = new();
    foreach (var subSprite in spriteSheet)
    {
      Sprites.Add(subSprite.name, subSprite);
    }

    Utils.Log($"[TechnicolorAssets]: Loaded {Sprites.Count} sprites", LogType.Loading);
  }
}
