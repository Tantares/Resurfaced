namespace Technicolor;

public abstract class ZoneDataBase
{
  [Persistent] public string Name;

  public EditorColorZone EditorZone => ZoneLibrary.EditorColorZones[Name];

  public string DisplayName => EditorZone.DisplayName;

  [Persistent(name = "PrimarySwatch")] protected string _primarySwatchName;
  [Persistent(name = "SecondarySwatch")] protected string _secondarySwatchName;

  public Swatch PrimarySwatch
  {
    get => SwatchLibrary.GetSwatch(_primarySwatchName);
    set => _primarySwatchName = value.Name;
  }

  public Swatch SecondarySwatch
  {
    get => SwatchLibrary.GetSwatch(_secondarySwatchName);
    set => _secondarySwatchName = value.Name;
  }
}
