using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;
using KSP.Localization;

namespace Technicolor
{
  public class TechnicolorEditorModes : MonoBehaviour
  {
    Toggle paintButton;
    Toggle fillButton;
    Toggle sampleButton;
    Toggle paletteButton;

    KFSMEvent on_goToModePaint;
    KFSMEvent on_paintSelect;
    KFSMEvent on_goToModeSample;
    KFSMEvent on_sampleSelect;
    KFSMEvent on_goToModeFill;
    KFSMEvent on_fillSelect;

    KFSMState st_paint_select;
    KFSMState st_sample_select;
    KFSMState st_fill_select;

    string PAINT_ENTER_MESSAGE = "";
    string PAINT_FAIL_MESSAGE = "";

    string SAMPLE_ENTER_MESSAGE = "";
    string SAMPLE_FAIL_MESSAGE = "";

    string FILL_ENTER_MESSAGE = "";

    KeyBinding paintKeyBinding;
    KeyBinding sampleKeyBinding;
    KeyBinding fillKeyBinding;
    KeyBinding paletteKeyBinding;

    EditorToolsUI editorToolsUI;

    const ConstructionMode paintConstructionMode = (ConstructionMode)5;
    const ConstructionMode sampleConstructionMode = (ConstructionMode)6;
    const ConstructionMode fillConstructionMode = (ConstructionMode)7;

    void Start()
    {
      PAINT_ENTER_MESSAGE = Localizer.Format("#LOC_Technicolor_UI_Message_Paint_Enter");
      PAINT_FAIL_MESSAGE = Localizer.Format("#LOC_Technicolor_UI_Message_Paint_Fail");
      SAMPLE_ENTER_MESSAGE = Localizer.Format("#LOC_Technicolor_UI_Message_Sample_Enter");
      SAMPLE_FAIL_MESSAGE = Localizer.Format("#LOC_Technicolor_UI_Message_Sample_Fail");
      FILL_ENTER_MESSAGE = Localizer.Format("#LOC_Technicolor_UI_Message_Fill_Enter");
      editorToolsUI = GetComponent<EditorToolsUI>();

      paintKeyBinding = new KeyBinding(Settings.PaintModeKey, ControlTypes.EDITOR_GIZMO_TOOLS | ControlTypes.KEYBOARDINPUT);
      sampleKeyBinding = new KeyBinding(Settings.SampleModeKey, ControlTypes.EDITOR_GIZMO_TOOLS | ControlTypes.KEYBOARDINPUT);
      fillKeyBinding = new KeyBinding(Settings.FillModeKey, ControlTypes.EDITOR_GIZMO_TOOLS | ControlTypes.KEYBOARDINPUT);
      paletteKeyBinding = new KeyBinding(Settings.TogglePaletteKey, ControlTypes.EDITOR_GIZMO_TOOLS | ControlTypes.KEYBOARDINPUT);

      CreateToolButtons();

      GameEvents.onEditorConstructionModeChange.Remove(EditorLogic.fetch.onConstructionModeChanged);
      GameEvents.onEditorConstructionModeChange.Add(onConstructionModeChanged);

      PatchEditorFSM();
    }

    void CreateToolButtons()
    {
      paintButton = AddToolButton(
        editorToolsUI.rootButton, 
        "paintButton",
        Localizer.Format("#LOC_Technicolor_UI_Tooltip_ToolPaint"), 
        "tech-btn-brush-off", 
        "tech-btn-brush-on",
        10f);
      sampleButton = AddToolButton(
        editorToolsUI.rootButton,
        "sampleButton",
        Localizer.Format("#LOC_Technicolor_UI_Tooltip_ToolSample"),
        "tech-btn-dropper-off",
        "tech-btn-dropper-on");
      fillButton = AddToolButton(
        editorToolsUI.rootButton, 
        "fillButton",
        Localizer.Format("#LOC_Technicolor_UI_Tooltip_ToolFill"),
        "tech-btn-bucket-off", 
        "tech-btn-bucket-on");
      paletteButton = AddToolButton(
        editorToolsUI.rootButton, 
        "paletteButton",
        Localizer.Format("#LOC_Technicolor_UI_Tooltip_ToolPalette"), 
        "tech-btn-palette-off", 
        "tech-btn-palette-on");
      paletteButton.group = null;

      paintButton.onValueChanged.AddListener(onPaintButtonInput);
      sampleButton.onValueChanged.AddListener(onSampleButtonInput);
      fillButton.onValueChanged.AddListener(onFillButtonInput);
      paletteButton.onValueChanged.AddListener(onPaletteButtonInput);
    }
    Toggle AddToolButton(Toggle template, string name, string tooltip, string spriteNameOff, string spriteNameOn, float extraPadding = 0f)
    {
      Toggle newButton = GameObject.Instantiate(template);
      RectTransform buttonTransform = newButton.transform as RectTransform;
      float dX = (editorToolsUI.rootButton.transform as RectTransform).anchoredPosition.x - (editorToolsUI.rotateButton.transform as RectTransform).anchoredPosition.x;
      Vector3 prevButtonPosition = (editorToolsUI.rootButton.transform.parent.GetChild(editorToolsUI.rootButton.transform.parent.childCount - 1) as RectTransform).anchoredPosition;
      prevButtonPosition.x += dX + extraPadding;
      buttonTransform.SetParent(editorToolsUI.rootButton.transform.parent, false);
      buttonTransform.anchoredPosition = prevButtonPosition;

      newButton.gameObject.name = name;
      newButton.GetComponent<TooltipController_Text>().SetText(tooltip);

      newButton.image.sprite = TechnicolorAssets.GetSprite(spriteNameOff);
      (newButton.graphic as Image).sprite = TechnicolorAssets.GetSprite(spriteNameOn);
      return newButton;
    }
    static KFSMCallback Combine(params KFSMCallback[] callbacks)
    {
      return (KFSMCallback)Delegate.Combine(callbacks);
    }
    Part selectedPart
    {
      get => EditorLogic.fetch.selectedPart;
      set { EditorLogic.fetch.selectedPart = value; }
    }
    void PatchEditorFSM()
    {
      KerbalFSM fsm = EditorLogic.fetch.fsm;
      int layerMask = EditorLogic.fetch.layerMask | 4 | 0x200000;

      // add states

      st_paint_select = new KFSMState("st_paint_select")
      {
        OnUpdate = Combine(
          EditorLogic.fetch.UndoRedoInputUpdate,
          EditorLogic.fetch.snapInputUpdate,
          EditorLogic.fetch.partSearchUpdate)
      };
      fsm.AddState(st_paint_select);
      st_sample_select = new KFSMState("st_sample_select")
      {
        OnUpdate = Combine(
         EditorLogic.fetch.UndoRedoInputUpdate,
         EditorLogic.fetch.snapInputUpdate,
         EditorLogic.fetch.partSearchUpdate)
      };
      fsm.AddState(st_sample_select);
      st_fill_select = new KFSMState("st_fill_select")
      {
        OnUpdate = Combine(
         EditorLogic.fetch.UndoRedoInputUpdate,
         EditorLogic.fetch.snapInputUpdate,
         EditorLogic.fetch.partSearchUpdate)
      };
      fsm.AddState(st_fill_select);
      // add events

      on_goToModePaint = new KFSMEvent("on_goToModePaint")
      {
        updateMode = KFSMUpdateMode.MANUAL_TRIGGER,
        OnEvent = delegate
        {
          ScreenMessages.PostScreenMessage(PAINT_ENTER_MESSAGE, EditorLogic.fetch.modeMsg);
          on_goToModePaint.GoToStateOnEvent = st_paint_select;
        }
      };
      fsm.AddEvent(on_goToModePaint,
        EditorLogic.fetch.st_idle,
        EditorLogic.fetch.st_offset_select,
        EditorLogic.fetch.st_offset_tweak,
        EditorLogic.fetch.st_rotate_select,
        EditorLogic.fetch.st_rotate_tweak,
        EditorLogic.fetch.st_root_unselected,
        EditorLogic.fetch.st_root_select,
        st_sample_select,
        st_fill_select
        );

      on_goToModeSample = new KFSMEvent("on_goToModeSample")
      {
        updateMode = KFSMUpdateMode.MANUAL_TRIGGER,
        OnEvent = delegate
        {
          ScreenMessages.PostScreenMessage(SAMPLE_ENTER_MESSAGE, EditorLogic.fetch.modeMsg);
          on_goToModeSample.GoToStateOnEvent = st_sample_select;
        }
      };
      fsm.AddEvent(on_goToModeSample,
        EditorLogic.fetch.st_idle,
        EditorLogic.fetch.st_offset_select,
        EditorLogic.fetch.st_offset_tweak,
        EditorLogic.fetch.st_rotate_select,
        EditorLogic.fetch.st_rotate_tweak,
        EditorLogic.fetch.st_root_unselected,
        EditorLogic.fetch.st_root_select,
        st_paint_select,
        st_fill_select); ;

      on_goToModeFill = new KFSMEvent("on_goToModeFill")
      {
        updateMode = KFSMUpdateMode.MANUAL_TRIGGER,
        OnEvent = delegate
        {
          ScreenMessages.PostScreenMessage(FILL_ENTER_MESSAGE, EditorLogic.fetch.modeMsg);
          on_goToModeFill.GoToStateOnEvent = st_fill_select;
        }
      };
      fsm.AddEvent(on_goToModeFill,
        EditorLogic.fetch.st_idle,
        EditorLogic.fetch.st_offset_select,
        EditorLogic.fetch.st_offset_tweak,
        EditorLogic.fetch.st_rotate_select,
        EditorLogic.fetch.st_rotate_tweak,
        EditorLogic.fetch.st_root_unselected,
        EditorLogic.fetch.st_root_select,
        st_paint_select,
        st_sample_select); ;

      on_paintSelect = new KFSMEvent("on_paintSelect")
      {
        updateMode = KFSMUpdateMode.UPDATE,
        OnCheckCondition = delegate
        {
          if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
          {
            var toPaint = EditorLogic.fetch.pickPart(layerMask, Input.GetKey(KeyCode.LeftShift), pickRootIfFrozen: false);
            if (toPaint != null)
            {
              if (!EditorLogic.fetch.ship.Contains(toPaint))
              {
                //on_paintSelect.GoToStateOnEvent = EditorLogic.fetch.st_place;
                //EditorLogic.fetch.on_partPicked.OnEvent();
                return false;
              }
              var module = toPaint.FindModuleImplementing<ModuleTechnicolor>();
              if (module == null)
              {
                ScreenMessages.PostScreenMessage(PAINT_FAIL_MESSAGE, 1, ScreenMessageStyle.LOWER_CENTER);

                return false;
              }
              else
              {
                module.SetPartSwatches(TechnicolorEditorLogic.SwatchData);
              }
              return false;
            }
          }

          return false;
        }
      };
      fsm.AddEvent(on_paintSelect, st_paint_select);

      on_sampleSelect = new KFSMEvent("on_sampleSelect")
      {
        updateMode = KFSMUpdateMode.UPDATE,
        OnCheckCondition = delegate
        {
          if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
          {
            var toSample = EditorLogic.fetch.pickPart(layerMask, Input.GetKey(KeyCode.LeftShift), pickRootIfFrozen: false);
            if (toSample != null)
            {
              if (!EditorLogic.fetch.ship.Contains(toSample))
              {
                on_sampleSelect.GoToStateOnEvent = EditorLogic.fetch.st_place;
                EditorLogic.fetch.on_partPicked.OnEvent();
                return false;
              }
              var module = toSample.FindModuleImplementing<ModuleTechnicolor>();
              if (module == null)
              {
                ScreenMessages.PostScreenMessage(SAMPLE_FAIL_MESSAGE, 1, ScreenMessageStyle.LOWER_CENTER);
                return false;
              }
              else
              {
                module.GetPartSwatches(TechnicolorEditorLogic.SwatchData);
                TechnicolorEditorLogic.SetSwatchesSampled();
              }
              return false;
            }
          }

          return false;
        }
      };
      fsm.AddEvent(on_sampleSelect, st_sample_select);
      on_fillSelect = new KFSMEvent("on_fillSelect")
      {
        updateMode = KFSMUpdateMode.UPDATE,
        OnCheckCondition = delegate
        {
          if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
          {
            var toPaint = EditorLogic.fetch.pickPart(layerMask, Input.GetKey(KeyCode.LeftShift), pickRootIfFrozen: false);
            if (toPaint != null)
            {
              if (!EditorLogic.fetch.ship.Contains(toPaint))
              {
                on_fillSelect.GoToStateOnEvent = EditorLogic.fetch.st_place;
                EditorLogic.fetch.on_partPicked.OnEvent();
                return false;
              }
              var module = toPaint.FindModuleImplementing<ModuleTechnicolor>();
              if (module != null)
              {
                module.SetPartSwatches(TechnicolorEditorLogic.SwatchData);
              }

              foreach (Part p in EditorLogic.FindPartsInChildren(toPaint))
              {
                module = p.FindModuleImplementing<ModuleTechnicolor>();
                if (module != null)
                {
                  module.SetPartSwatches(TechnicolorEditorLogic.SwatchData);
                }
              }

              

              return false;
            }
          }

          return false;
        }
      };
      fsm.AddEvent(on_fillSelect, st_fill_select);

      fsm.AddEvent(EditorLogic.fetch.on_goToModeRotate, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_goToModePlace, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_goToModeOffset, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_goToModeRoot, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_podDeleted, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_partCreated, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_partOverInventoryPAW, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_newShip, st_paint_select, st_sample_select, st_fill_select);
      fsm.AddEvent(EditorLogic.fetch.on_shipLoaded, st_paint_select, st_sample_select, st_fill_select);
    }

    void Update()
    {
      if (paintKeyBinding.GetKeyDown())
      {
        SetMode(paintConstructionMode, true);
      }
      if (sampleKeyBinding.GetKeyDown())
      {
        SetMode(sampleConstructionMode, true);
      }
      if (fillKeyBinding.GetKeyDown())
      {
        SetMode(fillConstructionMode, true);
      }
      if (paletteKeyBinding.GetKeyDown())
      {
        TechnicolorUI.Instance.MaterialWindow.ToggleVisible();
      }
    }

    private void onConstructionModeChanged(ConstructionMode mode)
    {
      if (mode == EditorLogic.fetch.constructionMode) return;

      if (mode == paintConstructionMode)
      {
        EditorLogic.fetch.coordSpaceBtn.gameObject.SetActive(value: false);
        EditorLogic.fetch.radialSymmetryBtn.gameObject.SetActive(value: false);

        EditorLogic.fetch.fsm.RunEvent(on_goToModePaint);

        EditorLogic.fetch.constructionMode = mode;
      }
      else if (mode == sampleConstructionMode)
      {

        EditorLogic.fetch.coordSpaceBtn.gameObject.SetActive(value: false);
        EditorLogic.fetch.radialSymmetryBtn.gameObject.SetActive(value: false);

        EditorLogic.fetch.fsm.RunEvent(on_goToModeSample);
        EditorLogic.fetch.constructionMode = mode;
      }
      else if (mode == fillConstructionMode)
      {

        EditorLogic.fetch.coordSpaceBtn.gameObject.SetActive(value: false);
        EditorLogic.fetch.radialSymmetryBtn.gameObject.SetActive(value: false);

        EditorLogic.fetch.fsm.RunEvent(on_goToModeFill);
        EditorLogic.fetch.constructionMode = mode;
      }
      else
      {
        EditorLogic.fetch.onConstructionModeChanged(mode);
      }
    }

    void OnDestroy()
    {
      paintButton.onValueChanged.RemoveListener(onPaintButtonInput);
      sampleButton.onValueChanged.RemoveListener(onSampleButtonInput);
      fillButton.onValueChanged.RemoveListener(onFillButtonInput);
      paletteButton.onValueChanged.RemoveListener(onPaletteButtonInput);

      GameEvents.onEditorConstructionModeChange.Remove(onConstructionModeChanged);
    }

    private void onPaintButtonInput(bool b)
    {
      if (b && paintButton.interactable)
      {
        SetMode(paintConstructionMode, false);
      }
    }
    private void onSampleButtonInput(bool b)
    {
      if (b && sampleButton.interactable)
      {
        SetMode(sampleConstructionMode, false);
      }
    }
    private void onFillButtonInput(bool b)
    {
      if (b && fillButton.interactable)
      {
        SetMode(fillConstructionMode, false);
      }
    }
    private void onPaletteButtonInput(bool b)
    {
      if (paletteButton.interactable)
      {
        TechnicolorUI.Instance.MaterialWindow.SetVisible(b);
      }
    }
    public void SetMode(ConstructionMode mode, bool updateUI)
    {
      editorToolsUI.SetMode(mode, updateUI);

      if (editorToolsUI.constructionMode == paintConstructionMode && updateUI)
      {
        paintButton.isOn = true;
      }
      if (editorToolsUI.constructionMode == sampleConstructionMode && updateUI)
      {
        sampleButton.isOn = true;
      }
      if (editorToolsUI.constructionMode == fillConstructionMode && updateUI)
      {
        fillButton.isOn = true;
      }
    }
  }
}