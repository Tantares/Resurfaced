using System;
using System.Collections.Generic;

namespace Technicolor
{
  public class TechnicolorSwatchData
  {
    public Dictionary<SwatchSlot, TechnicolorSwatch> Slots;

    public TechnicolorSwatchData() 
    {
      Slots = new();
      Slots.Add(SwatchSlot.APrimary, TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultPrimarySwatch));
      Slots.Add(SwatchSlot.BPrimary, TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultPrimarySwatch));
      Slots.Add(SwatchSlot.CPrimary, TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultPrimarySwatch));
      Slots.Add(SwatchSlot.ASecondary, TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultSecondarySwatch));
      Slots.Add(SwatchSlot.BSecondary, TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultSecondarySwatch));
      Slots.Add(SwatchSlot.CSecondary, TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultSecondarySwatch));
    }
    public void Save(ConfigNode node)
    {
      foreach (KeyValuePair<SwatchSlot, TechnicolorSwatch> kvp in Slots)
      {
        node.AddValue(kvp.Key.ToString(), kvp.Value.Name);
      }
    }
    public void Load(ConfigNode node)
    {
      Slots = new();
      foreach (ConfigNode.Value val in node.values.values)
      {
        Enum.TryParse(val.name, out SwatchSlot thisSlot);
        Slots.Add(thisSlot, TechnicolorData.Instance.SwatchLibrary.GetSwatch(val.value));
      }

    }
  }
}
