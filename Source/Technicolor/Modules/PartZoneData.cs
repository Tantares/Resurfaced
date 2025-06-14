using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technicolor;

public class PartZoneData : ZoneDataBase, IDisposable
{
  private ModuleTechnicolor _module;
  private Part _part => _module.part;
  private ColorZone _configZone => _module.ConfigZones[Name];

  private List<Renderer> _renderers = [];

  private readonly Shabby.DynamicProperties.Props _props = new(priority: 10);

  static PartZoneData()
  {
    Shabby.DynamicProperties.MaterialPropertyManager.RegisterPropertyNamesForDebugLogging(
      "_TC1Color",
      "_TC1MetalBlend",
      "_TC1Metalness",
      "_TC1SmoothBlend",
      "_TC1Smoothness",
      "_TC2Color",
      "_TC2MetalBlend",
      "_TC2Metalness",
      "_TC2SmoothBlend",
      "_TC2Smoothness");
  }

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

    RegisterRenderers();
  }

  public void SetTargetRenderers(List<Renderer> renderers)
  {
    _renderers = renderers;
    RegisterRenderers();
  }

  private void RegisterRenderers()
  {
    foreach (var renderer in _renderers)
    {
      Shabby.DynamicProperties.MaterialPropertyManager.Instance?.Set(renderer, _props);
    }
  }

  public void Refresh()
  {
    _props.SetColor(ShaderPropertyID._TC1Color, PrimarySwatch.Color);
    _props.SetFloat(ShaderPropertyID._TC1Metalness, PrimarySwatch.Metalness);
    _props.SetFloat(ShaderPropertyID._TC1Smoothness, PrimarySwatch.Smoothness);
    _props.SetFloat(ShaderPropertyID._TC1SmoothBlend, PrimarySwatch.SmoothBlend);
    _props.SetFloat(ShaderPropertyID._TC1MetalBlend, PrimarySwatch.MetalBlend);

    _props.SetColor(ShaderPropertyID._TC2Color, SecondarySwatch.Color);
    _props.SetFloat(ShaderPropertyID._TC2Metalness, SecondarySwatch.Metalness);
    _props.SetFloat(ShaderPropertyID._TC2Smoothness, SecondarySwatch.Smoothness);
    _props.SetFloat(ShaderPropertyID._TC2SmoothBlend, SecondarySwatch.SmoothBlend);
    _props.SetFloat(ShaderPropertyID._TC2MetalBlend, SecondarySwatch.MetalBlend);
  }

  public void Dispose() => _props.Dispose();
}
