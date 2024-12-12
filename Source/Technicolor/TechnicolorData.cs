using UnityEngine;

namespace Technicolor
{
  /// <summary>
  ///   Class to load shaders and config level data for the mod.
  /// </summary>
  [KSPAddon(KSPAddon.Startup.Instantly, true)]
  public class TechnicolorData : MonoBehaviour
  {
    public        bool          FirstLoad = true;
    public SwatchLibrary SwatchLibrary;
    public static TechnicolorData Instance { get; private set; }
    
    protected void Awake()
    {
      Instance = this;
      GameObject.DontDestroyOnLoad(gameObject);
    }

    public static void ModuleManagerPostLoad()
    {
      Settings.Load();
      Instance.SwatchLibrary = new();
      Instance.SwatchLibrary.Load();
    }
  }
}