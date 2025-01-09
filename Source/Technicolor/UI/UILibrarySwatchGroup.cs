using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor;

public class UILibrarySwatchGroup : MonoBehaviour
{
  public bool SwatchesShown { get; private set; }
  public TechnicolorSwatchGroup Group => _group;
  [SerializeField] protected Button _headerButton;
  [SerializeField] protected Text _headerText;
  [SerializeField] protected GameObject _swatches;
  [SerializeField] protected Transform _swatchParent;

  protected TechnicolorSwatchGroup _group;
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

  public void SetGroup(TechnicolorSwatchGroup group)
  {
    _group = group;
    SwatchesShown = true;
    _headerText.text = $"▶ {_group.DisplayName}";

    CreateGroupSwatches();
  }

  public void CreateGroupSwatches()
  {
    _swatchIcons = new();

    foreach (var swatch in SwatchLibrary.Swatches)
    {
      if (swatch.Group == Group.Name)
      {
        var newSwatchIcon = Instantiate(TechnicolorAssets.SwatchLibraryButtonPrefab);
        newSwatchIcon.transform.SetParent(_swatchParent, false);

        var newSwatch = newSwatchIcon.GetComponent<UILibrarySwatch>();
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
      _headerText.text = $"▼ {_group.DisplayName}";
    }
    else
    {
      _headerText.text = $"▶ {_group.DisplayName}";
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
    foreach (var libSwatch in _swatchIcons)
    {
      libSwatch.SetHighlightState(swatch.Name == libSwatch.Swatch.Name);
    }
  }
}
