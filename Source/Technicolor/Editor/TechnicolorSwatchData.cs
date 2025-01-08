using System;
using System.Collections.Generic;

namespace Technicolor
{
  public class TechnicolorSwatchData
  {
    public List<TechnicolorPersistentZoneData> Zones;

    public TechnicolorSwatchData()
    {
      Zones = new();
      foreach (EditorColorZone zone in TechnicolorData.Instance.ZoneLibrary.EditorColorZones)
      {
        Zones.Add(new(zone));
      }
    }
    public TechnicolorPersistentZoneData GetZone(string name)
    {
      for (int i = 0; i < Zones.Count; i++)
      {
        if (Zones[i].ZoneName == name)
          return Zones[i];
      }
      return null;
    }

    public void Save(ConfigNode node)
    {
      foreach (var zoneData in Zones)
      {
        node.AddNode(zoneData.Save());
      }
    }
    public void Load(ConfigNode node)
    {
      if (Zones == null)
        Zones = new();

      foreach (ConfigNode zoneNode in node.GetNodes("EDITOR_COLOR_ZONE"))
      {
        TechnicolorPersistentZoneData loadedData = new(zoneNode);
        if (Zones.Find(x => x.ZoneName == loadedData.ZoneName) == null)
        {
          Zones.Add(loadedData);
        }
      }

    }
  }
  public class TechnicolorPersistentZoneData
  {
    public string ZoneName = "main";
    public string DisplayName = "Main";
    public TechnicolorSwatch PrimarySwatch;
    public TechnicolorSwatch SecondarySwatch;

    public bool RestrictToMaterialGroups = true;
    public bool AutoApply = false;
    public bool ActiveInEditor = false;
    public bool AlwaysActive = false;

    public TechnicolorPersistentZoneData(EditorColorZone edZone)
    {
      ZoneName = edZone.Name;
      DisplayName = edZone.DisplayName;
      PrimarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultPrimarySwatch);
      SecondarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(Settings.DefaultSecondarySwatch);
      AlwaysActive = edZone.AlwaysActive;

      RestrictToMaterialGroups = edZone.RestrictToGroupsDefault;

      if (AlwaysActive)
      {
        ActiveInEditor = true;
      }
    }

    public TechnicolorPersistentZoneData(ConfigNode node)
    {
      Load(node);
    }

    public ConfigNode Save()
    {
      ConfigNode node = new ConfigNode("EDITOR_COLOR_ZONE");
      node.AddValue("name", ZoneName);
      node.AddValue("PrimarySwatch", PrimarySwatch.Name);
      node.AddValue("SecondarySwatch", SecondarySwatch.Name);
      node.AddValue("AlwaysActive", AlwaysActive);
      node.AddValue("RestrictToMaterialGroups", RestrictToMaterialGroups);
      return node;

    }
    public void Load(ConfigNode node)
    {
      string priSwatchName = "";
      string secSwatchName = "";
      node.TryGetValue("name", ref ZoneName);
      node.TryGetValue("PrimarySwatch", ref priSwatchName);
      node.TryGetValue("SecondarySwatch", ref secSwatchName);
      node.TryGetValue("AlwaysActive", ref AlwaysActive);
      node.TryGetValue("RestrictToMaterialGroups", ref RestrictToMaterialGroups);

      if (priSwatchName != "")
      {
        PrimarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(priSwatchName);
      }
      if (secSwatchName != "")
      {
        SecondarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(secSwatchName);
      }
      if (ZoneName == "main")
      {
        ActiveInEditor = true;
      }
    }
  }
}

