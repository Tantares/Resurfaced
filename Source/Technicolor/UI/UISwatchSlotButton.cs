using KSP.Localization;
using KSP.UI.TooltipTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor;

public class UISwatchSlotButton
{
  public SwatchSlot SwatchSlot { get; private set; }
  protected Toggle _swatchButton;
  protected Image _swatchImage;

  protected TooltipController_Text _tooltip;
  protected Transform _transform;
  protected UISwatchZoneWidget _parentWidget;

  public UISwatchSlotButton(Transform t, SwatchSlot slot, UISwatchZoneWidget widget)
  {
    SwatchSlot = slot;
    _parentWidget = widget;
    _transform = t;
    _swatchButton = _transform.GetComponent<Toggle>();
    _swatchImage = UIUtils.FindChildOfType<Image>("Image", _transform);
    _swatchButton.group = TechnicolorUI.Instance.MaterialWindow.SwatchButtonGroup;
    _swatchButton.onValueChanged.AddListener(delegate { OnSelectSlotToEdit(); });

    SetupTooltips(t, Tooltips.FindTextTooltipPrefab());
  }

  protected void SetupTooltips(Transform root, Tooltip_Text prefab)
  {
    if (SwatchSlot == SwatchSlot.Primary)
    {
      _tooltip = Tooltips.AddTooltip(root.gameObject,
                                     prefab,
                                     Localizer.Format("#LOC_Technicolor_UI_Tooltip_Primary"));
    }
    else
    {
      _tooltip = Tooltips.AddTooltip(root.gameObject,
                                     prefab,
                                     Localizer.Format("#LOC_Technicolor_UI_Tooltip_Secondary"));
    }
  }

  public Vector3 GetButtonPosition()
  {
    return _swatchButton.transform.position
         + new Vector3(_swatchButton.GetComponent<RectTransform>().sizeDelta.x * 2f, 0, 0);
  }

  public void OnSelectSlotToEdit()
  {
    _swatchButton.SetIsOnWithoutNotify(true);
    _parentWidget.SelectSlotToEdit(SwatchSlot);
  }

  public void OnSetSwatch(Swatch newSwatch)
  {
    newSwatch.GenerateThumbnail();
    if (SwatchSlot == SwatchSlot.Primary)
    {
      _swatchImage.sprite = newSwatch.ThumbnailLeft;
    }
    else
    {
      _swatchImage.sprite = newSwatch.ThumbnailRight;
    }

    _tooltip.textString = newSwatch.DisplayName;
    _swatchImage.color = Color.white;
  }
}
