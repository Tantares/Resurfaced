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
      foreach (EditorColorZone zone in TechnicolorData.ZoneLibrary.EditorColorZones)
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
 
}

