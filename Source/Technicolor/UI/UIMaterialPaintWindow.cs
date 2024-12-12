using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

namespace Technicolor
{
  public enum SwatchSlot
  {
    APrimary,
    BPrimary,
    CPrimary,
    ASecondary,
    BSecondary,
    CSecondary,
    None
  }

  public class UIMaterialPaintWindow : MonoBehaviour
  {
    public bool WindowShown { get; private set; }

    public SwatchSlot SelectedSlot = SwatchSlot.None;


    protected Text panelTitle;
    protected Button closeButton;

    protected Button paintButton;
    protected Button sampleButton;
    protected Button fillButton;
    protected ScrollRect swatchScrollRect;
    protected Transform swatchAreaBase;

    protected List<UISwatchGroup> swatchLibraryGroups;
    protected List<UISwatchSlotButton> slotButtons;

    protected bool panelOpen = false;
    protected RectTransform rect;

    public void Awake()
    {
      /// Components
      rect = GetComponent<RectTransform>();
      panelTitle = UIUtils.FindChildOfType<Text>("TitleText", transform);
      closeButton = UIUtils.FindChildOfType<Button>("CloseButton", transform);

      paintButton = UIUtils.FindChildOfType<Button>("ButtonPaint", transform);
      fillButton = UIUtils.FindChildOfType<Button>("ButtonPaintAll", transform);
      sampleButton = UIUtils.FindChildOfType<Button>("ButtonSample", transform);

      swatchScrollRect = UIUtils.FindChildOfType<ScrollRect>("SwatchLibrary", transform);
      swatchAreaBase = transform.FindDeepChild("ScrollGroup");

      /// Interaction Setup
      closeButton.onClick.AddListener(delegate { OnClickCloseButton(); });

      DraggableWindow dragger = gameObject.AddComponent<DraggableWindow>();
      dragger.target = transform;
    }

    public void Start()
    {
      panelTitle.text = Localizer.Format("#LOC_Technicolor_UI_MaterialWindow_Title");
      closeButton.gameObject.SetActive(false);

      // trust me i'm an engineer
      slotButtons = new();
      UISwatchSlotButton newButton;
      newButton = new(transform.FindDeepChild("Swatch1A"), this, SwatchSlot.APrimary);
      slotButtons.Add(newButton);
      newButton = new(transform.FindDeepChild("Swatch1B"), this, SwatchSlot.BPrimary);
      slotButtons.Add(newButton);
      newButton = new(transform.FindDeepChild("Swatch1C"), this, SwatchSlot.CPrimary);
      slotButtons.Add(newButton);
      newButton = new(transform.FindDeepChild("Swatch2A"), this, SwatchSlot.ASecondary);
      slotButtons.Add(newButton);
      newButton = new(transform.FindDeepChild("Swatch2B"), this, SwatchSlot.BSecondary);
      slotButtons.Add(newButton);
      newButton = new(transform.FindDeepChild("Swatch2C"), this, SwatchSlot.CSecondary);
      slotButtons.Add(newButton);

      //// If first load we'll select the first slot
      if (SelectedSlot == SwatchSlot.None)
      {
        slotButtons[0].OnSelectSlotToEdit();
      }

      ResetEditorUISwatches();

      swatchLibraryGroups = new();
      foreach (string groupName in TechnicolorData.Instance.SwatchLibrary.SwatchGroups)
      {
        GameObject newGO = Instantiate(TechnicolorAssets.SwatchGroupPrefab);
        newGO.transform.SetParent(swatchAreaBase, false);

        UISwatchGroup newGroup = newGO.GetComponent<UISwatchGroup>();
        newGroup.SetGroup(groupName);
        swatchLibraryGroups.Add(newGroup);
      }
    }

    public void SetVisible(bool state)
    {
      WindowShown = state;
      rect.gameObject.SetActive(state);
    }
    public void OnSelectSwatchSlot(UISwatchSlotButton selected)
    {
      // we clicked on a swatch slot, need to find it in the library UI, show that this slot is selected
      SelectedSlot = selected.SwatchSlot;
    }
    public void ToggleVisible()
    {
      SetVisible(!WindowShown);
    }
    public void OnClickCloseButton()
    {
      SetVisible(false);
    }
    public void OnSelectSwatch(TechnicolorSwatch newSwatch)
    {
      /// we clicked on a swatch and need to assign it to the current slot
      Utils.Log($"[UIMaterialPaintWindow]: Swatch {newSwatch.Name} was selected", LogType.UI);

      foreach (UISwatchSlotButton btn in slotButtons)
      {
        if (btn.SwatchSlot == SelectedSlot)
        {
          btn.OnSetSwatch(newSwatch);
        }
      }
      TechnicolorEditorLogic.SwatchData.Slots[SelectedSlot] = newSwatch;
    }
    public void ResetEditorUISwatches()
    {
      foreach (UISwatchSlotButton btn in slotButtons)
      {
        btn.OnSetSwatch(TechnicolorEditorLogic.SwatchData.Slots[btn.SwatchSlot]);
      }
    }

  }
}
