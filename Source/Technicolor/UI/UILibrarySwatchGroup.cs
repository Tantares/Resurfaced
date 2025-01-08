using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor
{
  public class UILibrarySwatchGroup: MonoBehaviour
  {
    public bool SwatchesShown { get; private set; }
    public string GroupName => _displayText;
    [SerializeField]
    protected Button _headerButton;
    [SerializeField]
    protected Text _headerText;
    [SerializeField]
    protected GameObject _swatches;
    [SerializeField]
    protected Transform _swatchParent;

    protected string _displayText;
    protected List<UILibrarySwatch> _swatchIcons;

    public void Awake()
    {
      _headerButton.onClick.AddListener(delegate { OnHeaderClick(); });

    }
    public void AssignReferences()
    {
      _headerButton = UIUtils.FindChildOfType<Button>("SwatchHeader", transform);
      _headerText = UIUtils.FindChildOfType<Text>("SwatchHeaderLabel", transform);

      _swatchParent = transform.FindDeepChild("SwatchContainer");
      _swatches = _swatchParent.gameObject;
    }

    public void SetGroup(string name)
    {
      _displayText = name;
      SwatchesShown = true;
      _headerText.text = $"▶ {_displayText}";

      CreateGroupSwatches(name);
    }
    public void CreateGroupSwatches(string name)
    {
      _swatchIcons = new();


      foreach (TechnicolorSwatch swatch in TechnicolorData.Instance.SwatchLibrary.Swatches)
      {
        if (swatch.Group == name)
        {
          GameObject newSwatchIcon = Instantiate(TechnicolorAssets.SwatchLibraryButtonPrefab);
          newSwatchIcon.transform.SetParent(_swatchParent, false);

          UILibrarySwatch newSwatch = newSwatchIcon.GetComponent<UILibrarySwatch>();
          newSwatch.AssignSwatch(swatch);
          _swatchIcons.Add(newSwatch);
        }
      }
    }

    public void SetSwatchesShown(bool state)
    {
      SwatchesShown = state;
      if (SwatchesShown)
      {
        _headerText.text = $"▼ {_displayText}";
      }
      else
      {
        _headerText.text = $"▶ {_displayText}";
      }
      _swatches.SetActive(SwatchesShown);
    }
    public void ToggleSwatchesShown() 
    {
      SetSwatchesShown(!SwatchesShown);
    }
    public void OnHeaderClick()
    {
      ToggleSwatchesShown();
    }
    public void HighlightSwatch(TechnicolorSwatch swatch)
    {
      foreach (UILibrarySwatch libSwatch in _swatchIcons)
      {
        libSwatch.SetHighlightState(swatch.Name == libSwatch.Swatch.Name);

      }
    }

  }

}
