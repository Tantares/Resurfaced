﻿using UnityEngine;
using KSP.UI;

namespace Technicolor
{
  [KSPAddon(KSPAddon.Startup.EditorAny, false)]
  public class TechnicolorUI : MonoBehaviour
  {
    public static TechnicolorUI Instance { get; private set; }
    public UIMaterialPaintWindow MaterialWindow { get; private set; }

    public void Awake()
    {
      Instance = this;
      Utils.Log("[TechnicolorUI]: Awake", LogType.UI);
    }

    /// <summary>
    /// Sets up the UI and creates the windows
    /// </summary>
    public void Start()
    {
      Utils.Log("[TechnicolorUI]: Start", LogType.UI);

      CreateWindow();
      foreach (TechnicolorSwatch s in TechnicolorData.Instance.SwatchLibrary.Swatches)
      {
        s.GenerateThumbnail();
      }
    }

    protected void CreateWindow()
    {
      Utils.Log("[TechnicolorUI]: Creating panel", LogType.UI);
      GameObject newUIPanel = (GameObject)Instantiate(TechnicolorAssets.SwatchWindowPrefab, Vector3.zero, Quaternion.identity);
      newUIPanel.transform.SetParent(UIMasterController.Instance.dialogCanvas.transform);
      newUIPanel.transform.localScale = Vector3.one;
      newUIPanel.transform.localPosition = new Vector3(0.5f, 0.75f,0f);

      MaterialWindow = newUIPanel.AddComponent<UIMaterialPaintWindow>();
      MaterialWindow.SetVisible(false);
    }
    protected void DestroyWindow()
    {
      Utils.Log("[TechnicolorUI]: Destroying panel", LogType.UI);
      if (MaterialWindow != null)
      {
        Destroy(MaterialWindow.gameObject);
      }
    }

    public void OnDestroy()
    {
      DestroyWindow();
    }
  }
}