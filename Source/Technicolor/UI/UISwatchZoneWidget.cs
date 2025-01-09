using KSP.Localization;
using KSP.UI.TooltipTypes;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor;

public class UISwatchZoneWidget : MonoBehaviour
{
  public bool SettingsVisible = false;
  public string zoneName = "";

  [SerializeField] protected Text _title;

  [SerializeField] protected Button _settingsButton;
  [SerializeField] protected GameObject _settingsPopup;

  [SerializeField] protected Toggle _autoToggle;
  [SerializeField] protected Toggle _restrictToggle;

  [SerializeField] protected Text _autoToggleText;
  [SerializeField] protected Text _restrictToggleText;
  [SerializeField] protected UISwatchSlotButton _primarySlotButton;
  [SerializeField] protected UISwatchSlotButton _secondarySlotButton;

  private void Start()
  {
    SetupTooltips(transform, Tooltips.FindTextTooltipPrefab());
    Localize();

    _settingsButton.onClick.AddListener(delegate { OnClickSettings(); });
    _settingsPopup.SetActive(false);

    _restrictToggle.onValueChanged.AddListener(delegate { OnToggleRestrictions(); });
    _autoToggle.onValueChanged.AddListener(delegate { OnToggleAutoApply(); });
  }

  public void AssignReferences()
  {
    _title = UIUtils.FindChildOfType<Text>("ZoneName", transform);

    _settingsPopup = transform.FindDeepChild("SettingsPopup").gameObject;
    _settingsButton = UIUtils.FindChildOfType<Button>("SettingsButton", transform);
    _autoToggle = UIUtils.FindChildOfType<Toggle>("AutomaticToggle", transform);
    _restrictToggle = UIUtils.FindChildOfType<Toggle>("GroupRestrictToggle", transform);

    _autoToggleText = UIUtils.FindChildOfType<Text>("AutomaticLabel", transform);
    _restrictToggleText = UIUtils.FindChildOfType<Text>("GroupRestrictLabel", transform);
  }

  protected void Localize()
  {
    _autoToggleText.text =
      Localizer.Format("#LOC_Technicolor_UI_SwatchSlotWidget_AutomaticColorsToggle_Title");
    _restrictToggleText.text =
      Localizer.Format("#LOC_Technicolor_UI_SwatchSlotWidget_RestrictColorsToggle_Title");
  }

  protected void SetupTooltips(Transform root, Tooltip_Text prefab)
  {
    Tooltips.AddTooltip(_settingsButton.gameObject,
                        prefab,
                        Localizer.Format("#LOC_Technicolor_UI_Tooltip_Settings"));
    Tooltips.AddTooltip(_autoToggle.gameObject,
                        prefab,
                        Localizer.Format("#LOC_Technicolor_UI_Tooltip_AutomaticColors"));
    Tooltips.AddTooltip(_restrictToggle.gameObject,
                        prefab,
                        Localizer.Format("#LOC_Technicolor_UI_Tooltip_RestrictColors"));
  }

  public Vector3 GetButtonPosition(SwatchSlot slot)
  {
    if (slot == SwatchSlot.Primary)
      return _primarySlotButton.GetButtonPosition();
    return _secondarySlotButton.GetButtonPosition();
  }

  public void OnClickSettings()
  {
    ToggleSettingsWindow();
  }

  public void ToggleSettingsWindow()
  {
    SetSettingsVisible(!SettingsVisible);
  }

  public void SetSettingsVisible(bool state)
  {
    SettingsVisible = state;
    _settingsPopup.SetActive(state);
    if (state)
    {
      /// close the material lib
      TechnicolorUI.Instance.MaterialWindow.SetLibraryVisible(false);
    }
  }

  public void AssignZoneData(EditorZoneData slotData)
  {
    zoneName = slotData.ZoneName;
    _title.text = slotData.DisplayName;

    _primarySlotButton = new(transform.FindDeepChild("Swatch1"), SwatchSlot.Primary, this);
    _secondarySlotButton = new(transform.FindDeepChild("Swatch2"), SwatchSlot.Secondary, this);

    _primarySlotButton.OnSetSwatch(slotData.PrimarySwatch);
    _secondarySlotButton.OnSetSwatch(slotData.SecondarySwatch);

    _restrictToggle.SetIsOnWithoutNotify(slotData.RestrictToMaterialGroups);
    _autoToggle.SetIsOnWithoutNotify(slotData.AutoApply);
  }

  public void SetVisible(bool state)
  {
    transform.gameObject.SetActive(state);
  }

  public void SelectSlotToEdit(SwatchSlot slotType)
  {
    SetSettingsVisible(false);
    TechnicolorUI.Instance.MaterialWindow.SelectZoneAndSlotForEdit(zoneName, slotType);
  }

  public void OnSelectSwatch(Swatch newSwatch, SwatchSlot slotType)
  {
    if (slotType == SwatchSlot.Primary)
      _primarySlotButton.OnSetSwatch(newSwatch);
    if (slotType == SwatchSlot.Secondary)
      _secondarySlotButton.OnSetSwatch(newSwatch);
  }

  public void OnSelectSwatch(Swatch primarySwatch, Swatch secondarySwatch)
  {
    _primarySlotButton.OnSetSwatch(primarySwatch);
    _secondarySlotButton.OnSetSwatch(secondarySwatch);
  }

  public void OnToggleAutoApply()
  {
    foreach (var swatch in TechnicolorEditorLogic.EditorData.Zones)
    {
      if (swatch.ZoneName == zoneName)
      {
        swatch.AutoApply = _autoToggle.isOn;
      }
    }
  }

  public void OnToggleRestrictions()
  {
    foreach (var swatch in TechnicolorEditorLogic.EditorData.Zones)
    {
      if (swatch.ZoneName == zoneName)
      {
        swatch.RestrictToMaterialGroups = _restrictToggle.isOn;
      }
    }
  }
}
