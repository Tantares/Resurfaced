using System.Collections.Generic;
using KSP.Localization;
using KSP.UI.TooltipTypes;
using UniLinq;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor;

public enum SwatchSlot
{
  Primary,
  Secondary,
  None
}

public class UIMaterialPaintWindow : MonoBehaviour
{
  public bool WindowShown { get; private set; }
  public bool LibraryShown { get; private set; }

  public ToggleGroup SwatchButtonGroup;

  protected string _currentZone = "";
  protected SwatchSlot _currentSlotType = SwatchSlot.None;

  protected Text _panelTitle;

  protected GameObject _libraryObj;
  protected ScrollRect _libraryScrollRect;
  protected Transform _libraryAreaBase;

  protected Transform _zonesBase;

  protected Button _zoneAddButton;
  protected Dropdown _zoneDropdown;

  protected List<UILibrarySwatchGroup> _swatchLibraryGroups;
  protected List<UISwatchZoneWidget> _zoneWidgets;

  protected bool _windowOpen = false;
  protected RectTransform _rect;

  public void Awake()
  {
    /// Components
    _rect = GetComponent<RectTransform>();
    _panelTitle = UIUtils.FindChildOfType<Text>("TitleText", transform);

    _libraryScrollRect = UIUtils.FindChildOfType<ScrollRect>("SwatchLibrary", transform);
    _libraryAreaBase = transform.FindDeepChild("ScrollGroup");
    _libraryObj = transform.FindDeepChild("LibraryArea").gameObject;

    _zoneDropdown = UIUtils.FindChildOfType<Dropdown>("ZoneDropdown", transform);
    _zoneAddButton = UIUtils.FindChildOfType<Button>("ZoneAddButton", transform);

    _zonesBase = transform.FindDeepChild("SwatchSelection");
    SwatchButtonGroup = gameObject.AddComponent<ToggleGroup>();
    SwatchButtonGroup.allowSwitchOff = true;
    var dragger = gameObject.AddComponent<DraggableWindow>();
    dragger.target = transform;
  }

  public void Start()
  {
    _panelTitle.text = Localizer.Format("#LOC_Technicolor_UI_MaterialWindow_Title");
    _zoneWidgets = new();

    foreach (var zoneData in TechnicolorEditorLogic.EditorData.Zones)
    {
      var widget = Instantiate(TechnicolorAssets.SwatchWidgetPrefab);
      widget.transform.SetParent(_zonesBase, false);
      widget.transform.localPosition = Vector3.zero;
      widget.transform.localRotation = Quaternion.identity;
      widget.transform.localScale = Vector3.one;

      var w = widget.GetComponent<UISwatchZoneWidget>();
      w.AssignZoneData(zoneData);
      _zoneWidgets.Add(w);
    }

    ResetEditorUISwatches();
    //_zoneAddButton.onClick.AddListener(delegate { OnAddZone(); });
    _zoneDropdown.AddOptions(TechnicolorEditorLogic.EditorData.Zones.Select(x => x.DisplayName)
                               .ToList());
    _zoneDropdown.onValueChanged.AddListener(delegate { OnAddZone(); });
    /// set up the library panel
    _swatchLibraryGroups = new();
    foreach (var group in SwatchLibrary.SwatchGroups.Values)
    {
      var newGO = Instantiate(TechnicolorAssets.SwatchLibraryGroupPrefab);
      newGO.transform.SetParent(_libraryAreaBase, false);

      var newGroup = newGO.GetComponent<UILibrarySwatchGroup>();
      newGroup.SetGroup(group);
      _swatchLibraryGroups.Add(newGroup);
    }

    SetLibraryVisible(false);
    SetupTooltips(transform, Tooltips.FindTextTooltipPrefab());
  }

  protected void SetupTooltips(Transform root, Tooltip_Text prefab)
  {
    Tooltips.AddTooltip(_zoneDropdown.gameObject,
                        prefab,
                        Localizer.Format("#LOC_Technicolor_UI_Tooltip_AddZone"));
  }

  public void SetVisible(bool state)
  {
    WindowShown = state;
    _rect.gameObject.SetActive(state);
  }

  public void ToggleVisible()
  {
    SetVisible(!WindowShown);
  }

  public void OnAddZone()
  {
    // Adding is actually just flipping the visible switch to true and refreshing the list
    foreach (var zoneData in TechnicolorEditorLogic.EditorData.Zones)
    {
      if (_zoneDropdown.options[_zoneDropdown.value].text == zoneData.DisplayName)
      {
        zoneData.ActiveInEditor = true;
      }
    }

    ResetEditorUISwatches();
  }

  public void ToggleLibraryWindow()
  {
    SetLibraryVisible(!LibraryShown);
  }

  public void SetLibraryVisible(bool state)
  {
    LibraryShown = state;
    _libraryObj.SetActive(state);
  }

  public void SelectZoneAndSlotForEdit(string zoneName, SwatchSlot slotType)
  {
    if (zoneName == _currentZone && slotType == _currentSlotType && LibraryShown)
    {
      SetLibraryVisible(false);
    }
    else
    {
      _currentSlotType = slotType;
      _currentZone = zoneName;
      foreach (var zoneData in TechnicolorEditorLogic.EditorData.Zones)
      {
        if (_currentZone == zoneData.Name)
        {
          ShowLibraryForZoneAndSlot(zoneData);
          PositionLibrary(zoneName, slotType);
        }
      }
    }
  }

  protected void PositionLibrary(string zoneName, SwatchSlot slotType)
  {
    foreach (var widget in _zoneWidgets)
    {
      // close settings
      widget.SetSettingsVisible(false);
      if (widget.zoneName == zoneName)
      {
        _libraryObj.transform.position = widget.GetButtonPosition(slotType);
      }
    }
  }

  protected void ShowLibraryForZoneAndSlot(EditorZoneData editorZoneData)
  {
    SetLibraryVisible(true);
    foreach (var group in _swatchLibraryGroups)
    {
      group.gameObject.SetActive(false);

      if (_currentSlotType == SwatchSlot.Primary)
        group.HighlightSwatch(editorZoneData.PrimarySwatch);
      else
        group.HighlightSwatch(editorZoneData.SecondarySwatch);
    }

    FilterLibraryGroups(editorZoneData);
  }

  protected void FilterLibraryGroups(EditorZoneData editorZoneData)
  {
    var validGroups = ZoneLibrary.GetValidGroupsForZone(editorZoneData.Name).ToList();
    foreach (var group in _swatchLibraryGroups)
    {
      if (!editorZoneData.RestrictToMaterialGroups)
      {
        group.gameObject.SetActive(true);
      }
      else
      {
        if (validGroups.Contains(group.Group.Name))
        {
          group.gameObject.SetActive(true);
        }
      }
    }
  }

  public void OnSelectSwatch(Swatch newSwatch)
  {
    // we clicked on a swatch and need to assign it to the current slot
    Utils.Log(
      $"[UIMaterialPaintWindow]: Swatch {newSwatch.Name} was assigned to {_currentZone} ({_currentSlotType})",
      LogType.UI);

    foreach (var widget in _zoneWidgets)
    {
      if (_currentZone == widget.zoneName)
      {
        widget.OnSelectSwatch(newSwatch, _currentSlotType);
      }
    }

    foreach (var zoneData in TechnicolorEditorLogic.EditorData.Zones)
    {
      if (_currentZone == zoneData.Name)
      {
        if (_currentSlotType == SwatchSlot.Primary)
          zoneData.PrimarySwatch = newSwatch;
        if (_currentSlotType == SwatchSlot.Secondary)
          zoneData.SecondarySwatch = newSwatch;
      }
    }

    foreach (var group in _swatchLibraryGroups)
    {
      group.HighlightSwatch(newSwatch);
    }
  }

  public void SetUISwatches()
  {
    ResetEditorUISwatches();
  }

  public void ResetEditorUISwatches()
  {
    foreach (var widget in _zoneWidgets)
    {
      foreach (var zoneData in TechnicolorEditorLogic.EditorData.Zones)
      {
        if (widget.zoneName == zoneData.Name)
        {
          Utils.Log($"[ModuleTechnicolor] Setting {widget.zoneName} to {zoneData.ActiveInEditor}",
                    LogType.Any);
          widget.SetVisible(zoneData.ActiveInEditor);
          if (zoneData.ActiveInEditor)
          {
            widget.OnSelectSwatch(zoneData.PrimarySwatch, zoneData.SecondarySwatch);
          }
        }
      }
    }
  }
}
