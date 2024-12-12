using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UniLinq;

namespace Technicolor
{
  /// <summary>
  /// Loads and holds references to Asset Bundled things
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class TechnicolorAssets : MonoBehaviour
  {
    public static GameObject SwatchWindowPrefab { get; private set; }
    public static GameObject SwatchGroupPrefab { get; private set; }
    public static GameObject SwatchButtonPrefab { get; private set; }

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
      GameObject.DontDestroyOnLoad(gameObject);
      Utils.Log("[TechnicolorAssets]: Loading Assets", LogType.Loading);
      AssetBundle prefabs = AssetBundle.LoadFromFile(Path.Combine(KSPUtil.ApplicationRootPath, ASSET_PATH));

      /// Get the Prefabs
      SwatchWindowPrefab = prefabs.LoadAsset("SwatchWindow") as GameObject;

      SwatchGroupPrefab = prefabs.LoadAsset("SwatchGroup") as GameObject;
      SwatchGroupPrefab.AddComponent<UISwatchGroup>().AssignReferences();
      SwatchButtonPrefab = prefabs.LoadAsset("SwatchButton") as GameObject;
      SwatchButtonPrefab.AddComponent<UISwatch>().AssignReferences();

      Utils.Log("[TechnicolorAssets]: Loaded UI Prefabs", LogType.Loading);

      SwatchRenderMultiPrefab = prefabs.LoadAsset("technicolor-swatch-multi") as GameObject;
      SwatchRenderSpherePrefab = prefabs.LoadAsset("technicolor-swatch-sphere") as GameObject;
      SwatchRenderSkybox = prefabs.LoadAsset("technicolor-swatch-skybox") as Material;
      SwatchRenderTexture = prefabs.LoadAsset("VAB_lr") as Texture2D;

      //SwatchRenderSkybox.SetTexture("_Tex", cubemap);

      //Debug.Log(SwatchRenderSkybox.mainTexture);

      /// Get the Sprite Atlas
      Sprite[] spriteSheet = prefabs.LoadAssetWithSubAssets<Sprite>(SPRITE_ATLAS_NAME);
      Sprites = new Dictionary<string, Sprite>();
      foreach (Sprite subSprite in spriteSheet)
      {
        Sprites.Add(subSprite.name, subSprite);
      }
      Utils.Log($"[TechnicolorAssets]: Loaded {Sprites.Count} sprites", LogType.Loading);
    }
  }
}
