using UnityEngine;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;

namespace Technicolor
{
  public class UISwatch: MonoBehaviour
  {
    public TechnicolorSwatch Swatch => _swatch;

    protected TechnicolorSwatch _swatch;
    [SerializeField]
    protected Button button;
    [SerializeField]
    protected Image image;
    [SerializeField]
    protected TooltipController_Text tooltip;


    public void Awake()
    {
      button.onClick.AddListener(delegate { OnButtonClick(); });
    }

    public void AssignReferences()
    {
      button = transform.GetComponent<Button>();
      image = UIUtils.FindChildOfType<Image>("Image", transform);
      tooltip = gameObject.AddComponent<TooltipController_Text>();   
      tooltip.RequireInteractable = false;
    }

    public void OnButtonClick()
    {
      TechnicolorUI.Instance.MaterialWindow.OnSelectSwatch(Swatch);
    }

    public void AssignSwatch(TechnicolorSwatch librarySwatch)
    {
      _swatch = librarySwatch;
      tooltip.prefab = UIUtils.FindTextTooltipPrefab();
      tooltip.textString = Swatch.DisplayName;
      //TechnicolorData.Instance.SwatchLibrary.GenerateThumbnails();
      GenerateSwatchVisual();
    }

    protected void GenerateSwatchVisual()
    {
      _swatch.GenerateThumbnail();
      image.sprite = Swatch.Thumbnail;
      //image.color = Swatch.Color;
    }
  }

}
