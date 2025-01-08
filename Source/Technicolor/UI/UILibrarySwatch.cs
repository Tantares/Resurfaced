using UnityEngine;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;

namespace Technicolor
{
  public class UILibrarySwatch: MonoBehaviour
  {
    public TechnicolorSwatch Swatch => _swatch;

    protected TechnicolorSwatch _swatch;
    [SerializeField]
    protected Button _button;
    [SerializeField]
    protected Image _image;
    [SerializeField]
    protected Image _highlight;
    [SerializeField]
    protected TooltipController_Text _tooltip;

    protected bool _isSelected = false;

    public void Awake()
    {
      _button.onClick.AddListener(delegate { OnButtonClick(); });
    }

    public void AssignReferences()
    {
      _button = transform.GetComponent<Button>();
      _image = UIUtils.FindChildOfType<Image>("Image", transform);
      _highlight = UIUtils.FindChildOfType<Image>("Highlight", transform);
      _tooltip = gameObject.AddComponent<TooltipController_Text>();   
      _tooltip.RequireInteractable = false;
    }

    public void SetHighlightState(bool state)
    {
      _isSelected = state;
      _highlight.gameObject.SetActive(state);
    }
    public void OnButtonClick()
    {
      TechnicolorUI.Instance.MaterialWindow.OnSelectSwatch(Swatch);
      SetHighlightState(true);
    }

    public void AssignSwatch(TechnicolorSwatch librarySwatch)
    {
      _swatch = librarySwatch;
      _tooltip.prefab = UIUtils.FindTextTooltipPrefab();
      _tooltip.textString = Swatch.DisplayName;
      //TechnicolorData.Instance.SwatchLibrary.GenerateThumbnails();
      GenerateSwatchVisual();
    }

    protected void GenerateSwatchVisual()
    {
      _swatch.GenerateThumbnail();
      _image.sprite = Swatch.Thumbnail;
      //image.color = Swatch.Color;
    }
  }

}
