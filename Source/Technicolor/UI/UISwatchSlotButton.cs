using UnityEngine;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;
using KSP.Localization;

namespace Technicolor
{
  public class UISwatchSlotButton
  {
    public SwatchSlot SwatchSlot { get; private set; }
    protected Toggle swatchButton;
    protected Image swatchImage;

    protected Transform transform;
    protected UIMaterialPaintWindow parentWindow;

    public UISwatchSlotButton(Transform t, UIMaterialPaintWindow baseWindow, SwatchSlot slot)
    {
      parentWindow = baseWindow;
      SwatchSlot = slot;
      transform = t;
      swatchButton = transform.GetComponent<Toggle>();
      swatchImage = UIUtils.FindChildOfType<Image>("Image", transform);

      SetupTooltips(t, Tooltips.FindTextTooltipPrefab());

      swatchButton.onValueChanged.AddListener(delegate { OnSelectSlotToEdit(); });
    }
    protected void SetupTooltips(Transform root, Tooltip_Text prefab)
    {
      string slotID = SwatchSlot.ToString();
      Tooltips.AddTooltip(root.gameObject, prefab,
        Localizer.Format("#LOC_Technicolor_UI_Tooltip_MaterialWindowSlot", slotID.Substring(1), slotID.Substring(0, 1)));
    }
    public void OnSelectSlotToEdit()
    {
      swatchButton.SetIsOnWithoutNotify(true);
      parentWindow.OnSelectSwatchSlot(this);
    }

    public void OnSetSwatch(TechnicolorSwatch newSwatch)
    {
      newSwatch.GenerateThumbnail();
      if (SwatchSlot == SwatchSlot.APrimary || SwatchSlot == SwatchSlot.BPrimary || SwatchSlot == SwatchSlot.CPrimary)
      {
        swatchImage.sprite = newSwatch.ThumbnailLeft;
      }
      else
      {
        swatchImage.sprite = newSwatch.ThumbnailRight;
      }
      swatchImage.color = Color.white;
    }
  }
}
