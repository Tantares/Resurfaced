using System.Collections.Generic;

namespace Technicolor;

public class EditorData
{
  public List<EditorZoneData> Zones;

  public EditorData()
  {
    Zones = new();
    foreach (var zone in ZoneLibrary.EditorColorZones.Values)
    {
      Zones.Add(new(zone));
    }
  }

  public EditorZoneData GetZone(string name)
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

    foreach (var zoneNode in node.GetNodes("EDITOR_COLOR_ZONE"))
    {
      EditorZoneData loadedData = new(zoneNode);
      if (Zones.Find(x => x.ZoneName == loadedData.ZoneName) == null)
      {
        Zones.Add(loadedData);
      }
    }
  }
}
