using UnityEngine;
using KSP.UI;
using KSP.UI.Screens;

namespace Technicolor
{
  public class TechnicolorEditorRollover : MonoBehaviour
  {
    public bool Visible { get; private set; }
    protected GameObject widget;
    protected Transform widgetXform;
    protected UIEditorRolloverPanel widgetPanel;
    protected Part _prevPart;
    protected void Start()
    {
      Utils.Log("[TechnicolorEditorRollover]: Creating panel", LogType.UI);
      widget = (GameObject)Instantiate(TechnicolorAssets.EditorRolloverPrefab, Vector3.zero, Quaternion.identity);
      widgetXform = widget.transform;
      widgetXform.SetParent(UIMasterController.Instance.dialogCanvas.transform);
      widgetXform.localScale = Vector3.one;
      widgetPanel = widget.GetComponent<UIEditorRolloverPanel>();

    }
    protected void Update()
    {
      if (!( EditorLogic.fetch.constructionMode == (ConstructionMode)5  || EditorLogic.fetch.constructionMode == (ConstructionMode)6  || EditorLogic.fetch.constructionMode == (ConstructionMode)7)
        || EditorLogic.RootPart == null 
        || (EditorPanels.Instance.IsMouseOver()))
      {
        SetVisibility(false);
        return;
      }

      Part part = null;
      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit rayHit))
      {
        part = rayHit.transform.GetComponent<Part>();
      }

      if (part != null)
      {
        ModuleTechnicolor module = part.GetComponent<ModuleTechnicolor>();
        if (module != null)
        {
          SetVisibility(true);

          widgetXform.position = GetWindowPosition();
          if (part != _prevPart)
          {
            string zones = "";
            foreach (ColorZone zn in module.zones)
            {
              zones += $" • {zn.ZoneName}\n";
            }
            widgetPanel.SetText(zones);
            _prevPart = part;
          }
        }
        else
        {
          SetVisibility(false);
        }
      }
      else
      {
        SetVisibility(false);
      }
    }
    protected void SetVisibility(bool state)
    {
      Visible = state;
      //Utils.Log($"[TechnicolorEditorRollover]: setting vis from {widget.activeSelf} to {state}", LogType.UI);
      if (widget.activeSelf != state)
      {
        widget.SetActive(state);
      }
    }
    protected Vector2 GetWindowPosition()
    {
      Canvas canvas = UIMasterController.Instance.dialogCanvas;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        Input.mousePosition, canvas.worldCamera,
        out Vector2 position);

      return canvas.transform.TransformPoint(position) + new Vector3(10f, -10f);
    }
  }

}
