using System.Linq;
using UnityEngine;

namespace Technicolor
{

  public static class SwatchRenderUtility
  {
    private static Camera swatchCamera;
    private static RenderTexture renderTexture;
    private static Texture2D swatchTexture;

    /// Commonly tweaked render params
    internal static readonly float CAM_FOV = 25f;
    internal static readonly int RENDER_LAYER = 8;
    internal static readonly float LIGHT_INTENSITY = 0.35f;
    internal static readonly float LIGHT_DIST = 10f;
    internal static readonly float LIGHT_ELEV_OFFSET = 65f;
    internal static readonly float LIGHT_AZ_OFFSET = 45f;

    internal static float GetCameraDistace(float size, float cameraFOV, int pixelWidth)
    {
      return Mathf.Max(1f, size) * cameraFOV / ((float)pixelWidth * 0.7f);
    }

    internal static void GenerateRenderTexture(int resolution)
    {
      if (renderTexture != null)
      {
        renderTexture.Release();
      }
      renderTexture = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
      renderTexture.Create();
      swatchTexture = new Texture2D(resolution, resolution, TextureFormat.ARGB32, mipChain: false);
    }

    internal static Texture2D RenderCamera(Camera cam, int width, int height, int depth, RenderTextureReadWrite rtReadWrite)
    {
      RenderTexture renderTexture = new RenderTexture(width, height, depth, RenderTextureFormat.ARGB32, rtReadWrite);
      renderTexture.Create();
      RenderTexture active = RenderTexture.active;
      RenderTexture.active = renderTexture;
      cam.targetTexture = renderTexture;
      cam.Render();
      Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: true);
      texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, recalculateMipMaps: false);
      texture2D.Apply();
      RenderTexture.active = active;
      cam.targetTexture = null;
      renderTexture.Release();
      UnityEngine.Object.DestroyImmediate(renderTexture);
      renderTexture = null;
      return texture2D;
    }
    internal static void PlaceObjectAtPointing(Transform t, float distance, float elevation, float azimuth, float pitch, float heading)
    {

      t.position = Quaternion.AngleAxis(azimuth, Vector3.up) * Quaternion.AngleAxis(elevation, Vector3.right) * (Vector3.back * distance);
      t.rotation = Quaternion.AngleAxis(heading, Vector3.up) * Quaternion.AngleAxis(pitch, Vector3.right);
    }

    public static Texture2D RenderSwatchThumbnail(GameObject swatchPrefab, int resolution, Material sky, TechnicolorSwatch swatch, float elevation = 0, float azimuth = 0, float pitch = 0, float hdg = 0, float fovFactor = 2f)
    {
      GameObject cameraObject = new GameObject("swatchCamera");
      swatchCamera = cameraObject.AddComponent<Camera>();
      swatchCamera.clearFlags = CameraClearFlags.Color;
      swatchCamera.backgroundColor = Color.clear;
      swatchCamera.fieldOfView = CAM_FOV;
      swatchCamera.cullingMask = (1 << RENDER_LAYER);
      swatchCamera.enabled = false;
      swatchCamera.orthographic = true;
      swatchCamera.orthographicSize = 0.5f;
      swatchCamera.allowHDR = false;

      Skybox skybox = cameraObject.AddComponent<Skybox>();
      skybox.material = sky;

      GameObject lightObject = new GameObject("swatchLight");
      Light light = lightObject.AddComponent<Light>();
      light.renderingLayerMask = (1 << RENDER_LAYER);
      light.type = LightType.Spot;
      light.range = 100f;
      light.intensity = LIGHT_INTENSITY;
      PlaceObjectAtPointing(lightObject.transform, LIGHT_DIST, elevation + LIGHT_ELEV_OFFSET, azimuth + LIGHT_AZ_OFFSET, pitch + LIGHT_ELEV_OFFSET, hdg + LIGHT_AZ_OFFSET);

      GameObject swatchObject = GameObject.Instantiate(swatchPrefab);
      Bounds bounds = new Bounds();
      var children = swatchObject.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
      Renderer r = null;
      Material m;
      foreach (var child in children)
      {
        child.layer = RENDER_LAYER;
        r = child.GetComponent<Renderer>();
        if (r != null)
        {
          bounds.Encapsulate(r.bounds);
        }
      }
      Vector3 size = bounds.size;
      m = r.sharedMaterial;
      m.SetColor(ShaderPropertyID._TC1Color, swatch.Color);
      m.SetFloat(ShaderPropertyID._TC1Metalness, swatch.Metalness);
      m.SetFloat(ShaderPropertyID._TC1Smoothness, swatch.Smoothness);
      m.SetFloat(ShaderPropertyID._TC1SmoothBlend, swatch.SmoothBlend);
      m.SetFloat(ShaderPropertyID._TC1MetalBlend, swatch.MetalBlend);

      float camDist = GetCameraDistace(Mathf.Max(Mathf.Max(size.x, size.y), size.z), CAM_FOV * fovFactor, resolution);
      PlaceObjectAtPointing(swatchCamera.transform, camDist, elevation, azimuth, pitch, hdg);
      swatchObject.transform.SetParent(swatchCamera.transform);
      swatchObject.layer = RENDER_LAYER;
      lightObject.transform.SetParent(swatchCamera.transform);
      swatchCamera.transform.Translate(0f, -1000f, -250f);
      if (swatchTexture == null)
      {
        GenerateRenderTexture(resolution);
      }
      swatchTexture = RenderCamera(swatchCamera, resolution*3, resolution, 24, RenderTextureReadWrite.Default);

      UnityEngine.Object.Destroy(cameraObject);
      UnityEngine.Object.Destroy(swatchObject);
      return swatchTexture;
    }

  }


}
