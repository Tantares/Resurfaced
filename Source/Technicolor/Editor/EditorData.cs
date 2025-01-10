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
    for (int i = Zones.Count; i-- > 0;)
    {
      if (Zones[i].ZoneName == name) return Zones[i];
    }

    return null;
  }

  public void Save(ConfigNode node)
  {
    foreach (var zoneData in Zones) node.AddNode(zoneData.Save());
  }

  public void Load(ConfigNode node)
  {
    foreach (var zoneNode in node.GetNodes("EDITOR_COLOR_ZONE"))
    {
      EditorZoneData zoneData = new(zoneNode);
      bool found = false;
      for (int i = Zones.Count; i-- > 0;)
      {
        if (Zones[i].ZoneName != zoneData.ZoneName) continue;
        Zones[i] = zoneData;
        found = true;
        break;
      }

      if (!found) Zones.Add(zoneData);
    }
  }
}
