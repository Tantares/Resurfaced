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
    public ZoneLibrary ZoneLibrary;
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

      Instance.ZoneLibrary = new();
      Instance.ZoneLibrary.Load();
    }

    //public Texture2D lalaLand;
    //private RenderTexture lala2;
    //public void Update()
    //{
    //  if (HighLogic.LoadedSceneIsGame && Input.GetKeyDown(KeyCode.P))
    //  {
    //    lalaLand = SkyCapture.SnapshotSkybox(1024, FlightGlobals.ActiveVessel.parts[0].transform);

    //    //var flightCamera = FlightCamera.fetch;
    //    //if (flightCamera != null)
    //    //{
    //    //  var reflectionProbe = flightCamera.reflectionProbe;
    //    //  if (reflectionProbe != null)
    //    //  {
           
    //    //    var probeComponent = reflectionProbe.probeComponent;

    //    //    RenderTexture probeTex = probeComponent.realtimeTexture;

    //    //    if (lala2 == null)
    //    //    {
    //    //      lala2 = new RenderTexture(probeTex.width, probeTex.height, 16, probeTex.graphicsFormat, probeTex.mipmapCount);
    //    //      lala2.dimension = UnityEngine.Rendering.TextureDimension.Cube;
    //    //      lala2.useMipMap = true;
    //    //    }


    //    //    lalaLand = new Texture2D(probeTex.width*6, probeTex.height, TextureFormat.RGBAFloat, false);
    //    //    //lalaLand.dimension = UnityEngine.Rendering.TextureDimension.Cube;
    //    //    //RenderTexture.active = probeTex;

    //    //    for (int i = 0; i < 6; i++)
    //    //    {
    //    //      Graphics.CopyTexture(probeTex, i, lala2, i);
    //    //      Graphics.SetRenderTarget(lala2, 0, (CubemapFace)i);
    //    //      lalaLand.ReadPixels(new Rect(0, 0, probeTex.width, probeTex.height), i * probeTex.width, 0);
    //    //    }

    //    //    //lalaLand.ReadPixels(new Rect(0, 0, probeTex.width, probeTex.height), 0, 0);
    //    //    lalaLand.Apply();
    //    //  }
    //    //}
    //  }
    //}
  }
}