using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Technicolor
{
  public static class SkyCapture
  {

    internal static GameObject _cameraGameObject = null;
    internal static GameObject _nearGameObject = null;
    internal static GameObject _farGameObject = null;
    internal static GameObject _scaledGameObject = null;
    internal static GameObject _galaxyGameObject = null;

    internal static Camera _nearCamera = null;
    internal static Camera _farCamera = null;
    internal static Camera _scaledCamera = null;
    internal static Camera _galaxyCamera = null;

    internal static RenderTexture _renderTextureColor;
    internal static RenderTexture _renderTextureDepth;

    internal static RenderTexture _displayTextureColor;
    internal static RenderTexture _displayTextureDepth;

    static string CAMERA_PREFIX = "Tantares_";
    static string GALAXY_CAMERA_NAME = "GalaxyCamera";
    static string SCALED_CAMERA_NAME = "Camera ScaledSpace";
    static string FAR_CAMERA_NAME = "Camera 01";//"UIMainCamera";
    static string NEAR_CAMERA_NAME = "Camera 00";

    internal static bool setupDone = false;
    public static Texture2D SnapshotSkybox(int resolution, Transform parent)
    {
      float cameraFieldOfView = 90f;
      if (!setupDone)
      {
        _cameraGameObject = new GameObject("ohno");
        _cameraGameObject.transform.SetParent(parent, false);
        _cameraGameObject.transform.localPosition = Vector3.zero;
        _cameraGameObject.transform.localRotation = Quaternion.identity;


        // Create the camera render texture.

        _renderTextureColor = new RenderTexture(resolution, resolution, 0);
        _renderTextureDepth = new RenderTexture(resolution, resolution, 24);
        _renderTextureColor.Create();
        _renderTextureDepth.Create();

        // Create the GUI render texture.

        _displayTextureColor = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RGB565);
        _displayTextureDepth = new RenderTexture(resolution, resolution, 24);
        _displayTextureColor.Create();
        _displayTextureDepth.Create();

        // Setup all the cameras.

        _nearGameObject = new GameObject();
        _farGameObject = new GameObject();
        _scaledGameObject = new GameObject();
        _galaxyGameObject = new GameObject();

        // Add the near camera.

        _nearCamera = _nearGameObject.AddComponent<Camera>();
        var nearCameraReference = UnityEngine.Camera.allCameras.FirstOrDefault(cam => cam.name == NEAR_CAMERA_NAME);
        if (nearCameraReference != null)
        {
          _nearCamera.CopyFrom(nearCameraReference);
          _nearCamera.name = CAMERA_PREFIX + NEAR_CAMERA_NAME;
          _nearCamera.enabled = false;
          _nearCamera.renderingPath = RenderingPath.Forward;
          _nearCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Default"));

          // The camera is attached to our object transform and does not move from there.

          _nearCamera.transform.parent = _cameraGameObject.transform;
          _nearCamera.transform.localPosition = Vector3.zero;
          _nearCamera.transform.localRotation = Quaternion.identity;

          _nearCamera.fieldOfView = cameraFieldOfView;
        }

        // Add the far camera.

        _farCamera = _farGameObject.AddComponent<Camera>();
        var farCameraReference = UnityEngine.Camera.allCameras.FirstOrDefault(cam => cam.name == FAR_CAMERA_NAME);
        if (farCameraReference != null)
        {
          _farCamera.CopyFrom(farCameraReference);
          _farCamera.name = CAMERA_PREFIX + FAR_CAMERA_NAME;
          _farCamera.enabled = false;
          _farCamera.renderingPath = RenderingPath.Forward;

          // The camera is attached to our object transform and does not move from there.

          _farCamera.transform.parent = _cameraGameObject.transform;
          _farCamera.transform.localPosition = Vector3.zero;
          _farCamera.transform.localRotation = Quaternion.identity;

          _farCamera.fieldOfView = cameraFieldOfView;
        }

        // Add the scaled camera.

        _scaledCamera = _scaledGameObject.AddComponent<Camera>();
        var scaledCameraReference = UnityEngine.Camera.allCameras.FirstOrDefault(cam => cam.name == SCALED_CAMERA_NAME);
        if (scaledCameraReference != null)
        {
          _scaledCamera.CopyFrom(scaledCameraReference);
          _scaledCamera.name = CAMERA_PREFIX + SCALED_CAMERA_NAME;
          _scaledCamera.enabled = false;
          _scaledCamera.renderingPath = RenderingPath.Forward;

          // Scaled cam has no parent.

          _scaledCamera.fieldOfView = cameraFieldOfView;
        }

        // Add the galaxy camera.

        _galaxyCamera = _galaxyGameObject.AddComponent<Camera>();
        var galaxyCameraReference = UnityEngine.Camera.allCameras.FirstOrDefault(cam => cam.name == GALAXY_CAMERA_NAME);
        if (galaxyCameraReference != null)
        {
          _galaxyCamera.CopyFrom(galaxyCameraReference);
          _galaxyCamera.name = CAMERA_PREFIX + GALAXY_CAMERA_NAME;
          _galaxyCamera.enabled = false;
          _galaxyCamera.renderingPath = RenderingPath.Forward;

          // Galaxy camera renders the galaxy skybox and is not 
          // actually moving, but only rotating to look at the galaxy cube.

          Transform galaxyRoot = GalaxyCubeControl.Instance.transform.parent;
          _galaxyCamera.transform.parent = galaxyRoot;
          _galaxyCamera.transform.localPosition = Vector3.zero;
          _galaxyCamera.transform.localRotation = Quaternion.identity;
        }
        setupDone = true;
      }
      //
      if (skyTexture == null)
      {
        skyTexture = new Texture2D(resolution * 6, resolution, TextureFormat.ARGB32, mipChain: false);
      }

      _cameraGameObject.transform.localRotation = Quaternion.identity;
      _cameraGameObject.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
      _scaledCamera.transform.position = ScaledSpace.LocalToScaledSpace(_cameraGameObject.transform.position);
      _scaledCamera.transform.rotation = _cameraGameObject.transform.rotation;
      _galaxyGameObject.transform.rotation = _cameraGameObject.transform.rotation;

      //cameraObject.transform.Rotate(new Vector3(0, 90, 0), Space.Self);


      int cubeFace = 0;

      
        skyTexture = CaptureImage(skyTexture, resolution, cameraFieldOfView, resolution * cubeFace);
      
      cubeFace++;

      _cameraGameObject.transform.localRotation = Quaternion.identity;
      _cameraGameObject.transform.Rotate(new Vector3(0, 270, 0), Space.Self);
      _scaledCamera.transform.position = ScaledSpace.LocalToScaledSpace(_cameraGameObject.transform.position);
      _scaledCamera.transform.rotation = _cameraGameObject.transform.rotation;
      _galaxyGameObject.transform.rotation = _cameraGameObject.transform.rotation;
      skyTexture = CaptureImage(skyTexture, resolution, cameraFieldOfView, resolution * cubeFace);


      cubeFace++;

      _cameraGameObject.transform.localRotation = Quaternion.identity;
      _cameraGameObject.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
      _scaledCamera.transform.position = ScaledSpace.LocalToScaledSpace(_cameraGameObject.transform.position);
      _scaledCamera.transform.rotation = _cameraGameObject.transform.rotation;
      _galaxyGameObject.transform.rotation = _cameraGameObject.transform.rotation;
      skyTexture = CaptureImage(skyTexture, resolution, cameraFieldOfView, resolution * cubeFace);

      cubeFace++;
      _cameraGameObject.transform.localRotation = Quaternion.identity;
      _cameraGameObject.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
      _scaledCamera.transform.position = ScaledSpace.LocalToScaledSpace(_cameraGameObject.transform.position);
      _scaledCamera.transform.rotation = _cameraGameObject.transform.rotation;
      _galaxyGameObject.transform.rotation = _cameraGameObject.transform.rotation;
      skyTexture = CaptureImage(skyTexture, resolution, cameraFieldOfView, resolution * cubeFace);



      cubeFace++;

      _cameraGameObject.transform.localRotation = Quaternion.identity;
      _cameraGameObject.transform.Rotate(new Vector3(0, 0, 0), Space.Self);
      _scaledCamera.transform.position = ScaledSpace.LocalToScaledSpace(_cameraGameObject.transform.position);
      _scaledCamera.transform.rotation = _cameraGameObject.transform.rotation;
      _galaxyGameObject.transform.rotation = _cameraGameObject.transform.rotation;
      skyTexture = CaptureImage(skyTexture, resolution, cameraFieldOfView, resolution * cubeFace);

      cubeFace++;

      _cameraGameObject.transform.localRotation = Quaternion.identity;
      _cameraGameObject.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
      _scaledCamera.transform.position = ScaledSpace.LocalToScaledSpace(_cameraGameObject.transform.position);
      _scaledCamera.transform.rotation = _cameraGameObject.transform.rotation;
      _galaxyGameObject.transform.rotation = _cameraGameObject.transform.rotation;
      skyTexture = CaptureImage(skyTexture, resolution, cameraFieldOfView, resolution * cubeFace);

      
      return skyTexture;
    }

    

    static Texture2D skyTexture; 

    internal static Texture2D CaptureImage(Texture2D image, int resolution, float cameraFieldOfView, int offset)
    {

      try
      {
       
        _nearCamera.enabled = true;
        if (GameSettings.GraphicsVersion != GameSettings.GraphicsType.D3D11)
          _farCamera.enabled = true;

        _scaledCamera.enabled = true;
        _galaxyCamera.enabled = true;

        // Switch the camera FOV.

        
          _nearCamera.fieldOfView = cameraFieldOfView;
          //if (_farCamera != null)
            _farCamera.fieldOfView = cameraFieldOfView;
          _scaledCamera.fieldOfView = cameraFieldOfView;

        // Render camera to texture.


        _renderTextureColor.DiscardContents(true, true);
        RenderTexture.active = _renderTextureColor;

        _nearCamera.SetTargetBuffers(_renderTextureColor.colorBuffer, _renderTextureDepth.depthBuffer);
        _farCamera.SetTargetBuffers(_renderTextureColor.colorBuffer, _renderTextureDepth.depthBuffer);
        _scaledCamera.SetTargetBuffers(_renderTextureColor.colorBuffer, _renderTextureDepth.depthBuffer);
        _galaxyCamera.SetTargetBuffers(_renderTextureColor.colorBuffer, _renderTextureDepth.depthBuffer);

        _galaxyCamera.Render();
        _scaledCamera.Render();
        if (GameSettings.GraphicsVersion != GameSettings.GraphicsType.D3D11)
          _farCamera.Render();
        _nearCamera.Render();

        image.ReadPixels(new Rect(0, 0, resolution, resolution), offset, 0);
        image.Apply();

        RenderTexture.active = null;
        _nearCamera.targetTexture = null;
        _farCamera.targetTexture = null;
        _scaledCamera.targetTexture = null;
        _galaxyCamera.targetTexture = null;

        // Switch the camera off.


        _nearCamera.enabled = false;
        _farCamera.enabled = false;
        _scaledCamera.enabled = false;
        _galaxyCamera.enabled = false;
        return image;

      }
      catch (Exception ex)
      {
        return null;
      }
    }
  }
}
