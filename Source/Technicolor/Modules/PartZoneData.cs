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

    if (_configZone.Transforms.Count == 0)
    {
      // No specified transforms; gather renderers from the entire part.
      _renderers.AddRange(_part.GetComponentsInChildren<Renderer>(true));
    }
    else
    {
      // Only gather renderers parented to the specified transforms.
      foreach (var transform in _part.GetComponentsInChildren<Transform>(true))
      {
        if (!_configZone.Transforms.Contains(transform.name)) continue;
        _renderers.AddRange(transform.GetComponentsInChildren<Renderer>(true));
      }
    }
  }

  public void Apply(MaterialPropertyBlock mpb)
  {
    foreach (var renderer in _renderers)
    {
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

      renderer.SetPropertyBlock(mpb);
    }
  }
}
