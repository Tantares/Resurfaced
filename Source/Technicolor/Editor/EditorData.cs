using System.Collections.Generic;
using System.Linq;

namespace Technicolor;

public class EditorData
{
  public readonly List<EditorZoneData> Zones;

  public EditorData()
  {
    Zones = ZoneLibrary.EditorColorZones.Values.Select(zone => new EditorZoneData(zone)).ToList();
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
