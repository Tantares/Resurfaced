using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Technicolor
{
  public class UISwatchGroup: MonoBehaviour
  {
    public bool SwatchesShown { get; private set; }

    [SerializeField]
    protected Button headerButton;
    [SerializeField]
    protected Text headerText;
    [SerializeField]
    protected GameObject swatches;
    [SerializeField]
    protected Transform swatchParent;

    protected string displayText;
    protected List<UISwatch> swatchIcons;

    public void Awake()
    {
      headerButton.onClick.AddListener(delegate { OnHeaderClick(); });

    }
    public void AssignReferences()
    {
      headerButton = UIUtils.FindChildOfType<Button>("SwatchHeader", transform);
      headerText = UIUtils.FindChildOfType<Text>("SwatchHeaderLabel", transform);

      swatchParent = transform.FindDeepChild("SwatchContainer");
      swatches = swatchParent.gameObject;
    }

    public void SetGroup(string name)
    {
      displayText = name;
      SwatchesShown = true;
      headerText.text = $"▶ {displayText}";

      CreateGroupSwatches(name);
    }
    public void CreateGroupSwatches(string name)
    {
      swatchIcons = new();

      foreach (TechnicolorSwatch swatch in TechnicolorData.Instance.SwatchLibrary.Swatches)
      {
        if (swatch.Group == name)
        {
          GameObject newSwatchIcon = Instantiate(TechnicolorAssets.SwatchButtonPrefab);
          newSwatchIcon.transform.SetParent(swatchParent, false);

          UISwatch newSwatch = newSwatchIcon.GetComponent<UISwatch>();
          newSwatch.AssignSwatch(swatch);
          swatchIcons.Add(newSwatch);
        }
      }
    }
    public void SetSwatchesShown(bool state)
    {
      SwatchesShown = state;
      if (SwatchesShown)
      {
        headerText.text = $"▼ {displayText}";
      }
      else
      {
        headerText.text = $"▶ {displayText}";
      }
      swatches.SetActive(SwatchesShown);
    }
    public void ToggleSwatchesShown() 
    {
      SetSwatchesShown(!SwatchesShown);
    }
    public void OnHeaderClick()
    {
      ToggleSwatchesShown();
    }

  }
}
