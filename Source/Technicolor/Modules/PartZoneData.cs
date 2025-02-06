using System.Collections.Generic;
using UnityEngine;

namespace Technicolor;

public class PartZoneData : ZoneDataBase
{
  private ModuleTechnicolor _module;
  private Part _part => _module.part;
  private ColorZone _configZone => _module.ConfigZones[Name];

  private readonly List<Renderer> _renderers = [];

  public PartZoneData(ModuleTechnicolor module, string name)
  {
    _module = module;
    Name = name;
    _primarySwatchName = _configZone.DefaultPrimarySwatchName;
    _secondarySwatchName = _configZone.DefaultSecondarySwatchName;
  }

  public PartZoneData(ModuleTechnicolor module, ConfigNode node)
  {
    _module = module;
    ConfigNode.LoadObjectFromConfig(this, node);
  }

  public void Save(ConfigNode node)
  {
    ConfigNode.CreateConfigFromObject(this, node);
  }

  public void SetSwatches(EditorZoneData editorZoneData)
  {
    PrimarySwatch = editorZoneData.PrimarySwatch;
    SecondarySwatch = editorZoneData.SecondarySwatch;
  }

  public void FindTargetRenderers()
  {
    _renderers.Clear();

    bool blanket = _configZone.Transforms.Count == 0;
    foreach (var renderer in _part.GetComponentsInChildren<Renderer>(true))
    {
      if (blanket || _configZone.Transforms.Contains(renderer.transform.name))
        _renderers.Add(renderer);
    }
  }

  public void Apply(MaterialPropertyBlock mpb)
  {
    MaterialPropertyBlock rendererMPB = new();
    for (int i = 0; i < _renderers.Count; i++)
    {
      _renderers[i].GetPropertyBlock(rendererMPB);
      mpb.SetColor(ShaderPropertyID._EmissiveColor, rendererMPB.GetColor(ShaderPropertyID._EmissiveColor));

      mpb.SetColor(ShaderPropertyID._TC1Color, PrimarySwatch.Color);
      mpb.SetFloat(ShaderPropertyID._TC1Metalness, PrimarySwatch.Metalness);
      mpb.SetFloat(ShaderPropertyID._TC1Smoothness, PrimarySwatch.Smoothness);
      mpb.SetFloat(ShaderPropertyID._TC1SmoothBlend, PrimarySwatch.SmoothBlend);
      mpb.SetFloat(ShaderPropertyID._TC1MetalBlend, PrimarySwatch.MetalBlend);

      mpb.SetColor(ShaderPropertyID._TC2Color, SecondarySwatch.Color);
      mpb.SetFloat(ShaderPropertyID._TC2Metalness, SecondarySwatch.Metalness);
      mpb.SetFloat(ShaderPropertyID._TC2Smoothness, SecondarySwatch.Smoothness);
      mpb.SetFloat(ShaderPropertyID._TC2SmoothBlend, SecondarySwatch.SmoothBlend);
      mpb.SetFloat(ShaderPropertyID._TC2MetalBlend, SecondarySwatch.MetalBlend);

      _renderers[i].SetPropertyBlock(mpb);
    }
  }
}
