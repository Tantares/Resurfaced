using System.Collections.Generic;
using UnityEngine;

namespace Technicolor
{
  public class ModuleTechnicolor : PartModule
  {
    [SerializeField]
    public ColorZone[] zones;
    void Awake()
    {
      base.Awake();

    }
    public override void OnStart(StartState state)
    {
      base.OnStart(state);
      if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
      {
        if (zones != null)
        {
          ColorZone[] copyTarget = new ColorZone[zones.Length];
          for (int i = 0; i < copyTarget.Length; i++)
          {
            copyTarget[i] = Instantiate(zones[i]);
          }
          zones = copyTarget;
          for (int i = 0; i < zones.Length; i++)
          {
            zones[i].Initialize(part);
          }
        }
      }
    }
    public override void OnLoad(ConfigNode node)
    {
      base.OnLoad(node);

      var zoneNodes = node.GetNodes(TechnicolorConstants.MODULE_COLOR_NODE);

      int zoneCount = Mathf.Max(1, zoneNodes.Length);
      zones = new ColorZone[zoneCount];
      for (int i = 0; i < zoneNodes.Length; i++)
      {
        ColorZone zn = ScriptableObject.CreateInstance<ColorZone>();
        zn.Load(zoneNodes[i]);
        zones[i] = zn;
      }
      if (zoneNodes.Length == 0)
      {
        ColorZone zn = ScriptableObject.CreateInstance<ColorZone>();
        zones[0] = zn;
      }


    }
    public override void OnSave(ConfigNode node)
    {
      base.OnSave(node);
      
      if (zones != null)
      {
        for (int i = 0; i < zones.Length; i++)
        {
          ConfigNode zoneNode = new ConfigNode(TechnicolorConstants.MODULE_COLOR_NODE);
          zones[i].Save(zoneNode);
          node.AddNode(zoneNode);
        }
      }
    }

    public void SetPartSwatches(TechnicolorSwatchData swatches)
    {
      Utils.Log("[ModuleTechnicolor] Applying Swatches To Part", LogType.Any);
      /// this sucks rewrite it
      foreach (KeyValuePair<SwatchSlot, TechnicolorSwatch> kvp in swatches.Slots)
      {
        if (kvp.Key == SwatchSlot.APrimary || kvp.Key == SwatchSlot.ASecondary)
        {
          zones[0].SetSwatch(swatches.Slots[SwatchSlot.APrimary], swatches.Slots[SwatchSlot.ASecondary]);
        }
        if (zones.Length > 1)
        {
          if (kvp.Key == SwatchSlot.BPrimary || kvp.Key == SwatchSlot.BSecondary)
          {
            zones[1].SetSwatch(swatches.Slots[SwatchSlot.BPrimary], swatches.Slots[SwatchSlot.BSecondary]);
          }
        }
        if (zones.Length > 2)
        {
          if (kvp.Key == SwatchSlot.CPrimary || kvp.Key == SwatchSlot.CSecondary)
          {
            zones[2].SetSwatch(swatches.Slots[SwatchSlot.CPrimary], swatches.Slots[SwatchSlot.CSecondary]);
          }
        }
      }

      ApplySwatches();
    }

    public void GetPartSwatches(TechnicolorSwatchData swatches)
    {
      swatches.Slots[SwatchSlot.APrimary] = zones[0].PrimarySwatch ?? swatches.Slots[SwatchSlot.APrimary];
      swatches.Slots[SwatchSlot.ASecondary] = zones[0].SecondarySwatch ?? swatches.Slots[SwatchSlot.ASecondary];
      if (zones.Length > 1)
      {
        swatches.Slots[SwatchSlot.BPrimary] = zones[1].PrimarySwatch! ?? swatches.Slots[SwatchSlot.BPrimary];
        swatches.Slots[SwatchSlot.BSecondary] = zones[1].SecondarySwatch ?? swatches.Slots[SwatchSlot.BSecondary];
      }
      if (zones.Length > 2)
      {
        swatches.Slots[SwatchSlot.CPrimary] = zones[2].PrimarySwatch ?? swatches.Slots[SwatchSlot.CPrimary];
        swatches.Slots[SwatchSlot.CSecondary] = zones[2].SecondarySwatch ?? swatches.Slots[SwatchSlot.CSecondary];
      }
    }

    public void ApplySwatches()
    {
      
      for (int i = 0; i < zones.Length; i++)
      {
        zones[i].Apply();
      }
    }
  }
}
