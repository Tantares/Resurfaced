using System.Collections.Generic;
using UnityEngine;

namespace Technicolor
{
  public class ModuleTechnicolor : PartModule
  {
    [KSPField(isPersistant = true)]
    public bool FirstPlaced = true;

    [SerializeField]
    public ColorZone[] zones;

    public override void OnStart(StartState state)
    {
      base.OnStart(state);
      if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
      {
        if (zones != null)
        {
          if (FirstPlaced)
          {

            Utils.Log($"[ModuleTechnicolor] Collecting initial info from object", LogType.Any);
            // Copy out of the scriptable object
            ColorZone[] copyTarget = new ColorZone[zones.Length];
            for (int i = 0; i < copyTarget.Length; i++)
            {
              copyTarget[i] = Instantiate(zones[i]);
            }
            zones = copyTarget;

          }

          for (int i = 0; i < zones.Length; i++)
          {
            if (FirstPlaced)
            {
              TechnicolorPersistentZoneData z = TechnicolorEditorLogic.SwatchData.GetZone(zones[i].ZoneName);
              if (z.AutoApply)
              {
                zones[i].Initialize(part, z.PrimarySwatch.Name, z.SecondarySwatch.Name);
              }
              else
              {
                zones[i].Initialize(part);
              }
            }
            else
            {
              //zones[i].Initialize(part);
            }
          }
          FirstPlaced = false;
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
      foreach (TechnicolorPersistentZoneData zoneData in swatches.Zones)
      {
        if (zoneData.ActiveInEditor)
        {
          SetSwatchesForSlot(zoneData);
        }
      }
    }

    public void SetSwatchesForSlot(TechnicolorPersistentZoneData slotData)
    {
      Utils.Log($"[ModuleTechnicolor] Applying swatches for zone {slotData.ZoneName} To part", LogType.Any);

      for (int i = 0; i < zones.Length; i++)
      {
        if (zones[i].ZoneName == slotData.ZoneName)
        {
          zones[i].SetSwatch(slotData.PrimarySwatch, slotData.SecondarySwatch);
          zones[i].Apply();
        }
      }
    }

    public void GetPartSwatches(ref TechnicolorSwatchData swatches)
    {

      foreach (TechnicolorPersistentZoneData zoneData in swatches.Zones)
      {
        if (!zoneData.AlwaysActive)
          zoneData.ActiveInEditor = false;

        for (int i = 0; i < zones.Length; i++)
        {
          if (zones[i].ZoneName == zoneData.ZoneName)
          {

            Utils.Log($"[ModuleTechnicolor] Sampling {zones[i].PrimarySwatch.Name} and {zones[i].SecondarySwatch.Name} from {zones[i].ZoneName}", LogType.Any);
            zoneData.ActiveInEditor = true;
            zoneData.PrimarySwatch = zones[i].PrimarySwatch ?? zoneData.PrimarySwatch;
            zoneData.SecondarySwatch = zones[i].SecondarySwatch ?? zoneData.SecondarySwatch;
          }
        }
      }
    }

    public void ApplySwatches()
    {
      if (zones != null)
        for (int i = 0; i < zones.Length; i++)
        {
          zones[i].Apply();
        }
    }
  }
}
