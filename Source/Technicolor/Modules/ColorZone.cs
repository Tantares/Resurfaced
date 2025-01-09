using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Technicolor;

[Serializable]
public class ColorZone : ScriptableObject
{
  public Swatch PrimarySwatch => _primarySwatch;
  public Swatch SecondarySwatch => _secondarySwatch;

  [SerializeField] public string ZoneName = "main";
  [SerializeField] public string PrimarySwatchName = "";
  [SerializeField] public string SecondarySwatchName = "";
  [SerializeField] private string[] _transforms;

  private Swatch _primarySwatch;
  private Swatch _secondarySwatch;

  private Part _part;
  private Material[] _materials;
  private Renderer[] _renderers;
  public ColorZone() { }

  public ColorZone(ConfigNode node)
  {
    Load(node);
  }

  public void Save(ConfigNode node)
  {
    node.AddValue("name", ZoneName);
    node.AddValue("swatchPrimary", PrimarySwatchName);
    node.AddValue("swatchSecondary", SecondarySwatchName);
  }

  public void Load(ConfigNode node)
  {
    node.TryGetValue("name", ref ZoneName);
    node.TryGetValue("swatchPrimary", ref PrimarySwatchName);
    node.TryGetValue("swatchSecondary", ref SecondarySwatchName);
    _transforms = node.GetValues("transform");
  }

  public void Initialize(Part p)
  {
    _part = p;
    CollectMaterials();
    _primarySwatch = SwatchLibrary.GetSwatch(PrimarySwatchName);
    _secondarySwatch = SwatchLibrary.GetSwatch(SecondarySwatchName);
    //Apply();
  }

  public void Initialize(Part p, string overridePrimary, string overrideSecondary)
  {
    PrimarySwatchName = overridePrimary;
    SecondarySwatchName = overrideSecondary;
    Initialize(p);
  }

  protected void CollectMaterials()
  {
    /// Collect the renderers
    List<Renderer> renderers = new();
    /// If there are transform names, pick from them.
    if (_transforms != null && _transforms.Length > 0)
    {
      var candidates = _part.GetComponentsInChildren<Renderer>(true);
      for (int i = 0; i < _transforms.Length; i++)
      {
        for (int j = 0; j < candidates.Length; j++)
        {
          if (candidates[j].name == _transforms[i])
          {
            renderers.Add(candidates[j]);
          }
        }
      }
    }
    else
      /// Else fire everthing
    {
      renderers = _part.GetComponentsInChildren<Renderer>(true).ToList();
    }

    _renderers = renderers.ToArray();

    List<Material> mats = new();
    for (int i = 0; i < renderers.Count; i++)
    {
      if (renderers[i].material.shader.name == Constants.TEAMCOLOR_SHADER_NAME)
      {
        mats.Add(renderers[i].material);
      }
    }

    _materials = mats.ToArray();
  }

  public void SetSwatch(string primaryName, string secondaryName)
  {
    PrimarySwatchName = primaryName;
    SecondarySwatchName = secondaryName;

    _primarySwatch = SwatchLibrary.GetSwatch(primaryName);
    _secondarySwatch = SwatchLibrary.GetSwatch(secondaryName);
  }

  public void SetSwatch(Swatch primary, Swatch secondary)
  {
    PrimarySwatchName = primary.Name;
    SecondarySwatchName = secondary.Name;

    _primarySwatch = primary;
    _secondarySwatch = secondary;
  }

  public void Apply(MaterialPropertyBlock mpb)
  {
    //foreach (var kvp in stackRenderersCache)
    //{
    //  var anchor = kvp.Key;
    //  if (anchor.name == SkinStackAnchorName && transparentSkin)
    //    mpb.SetFloat(PropertyIDs._Opacity, transparentSkinOpacity);
    //  foreach (var renderer in kvp.Value) renderer.SetPropertyBlock(mpb);
    //  mpb.SetFloat(PropertyIDs._Opacity, partMPBProps.Opacity);
    for (int i = 0; i < _renderers.Length; i++)
    {
      if (_primarySwatch != null)
      {
        mpb.SetColor(ShaderPropertyID._TC1Color, _primarySwatch.Color);
        mpb.SetFloat(ShaderPropertyID._TC1Metalness, _primarySwatch.Metalness);
        mpb.SetFloat(ShaderPropertyID._TC1Smoothness, _primarySwatch.Smoothness);
        mpb.SetFloat(ShaderPropertyID._TC1SmoothBlend, _primarySwatch.SmoothBlend);
        mpb.SetFloat(ShaderPropertyID._TC1MetalBlend, _primarySwatch.MetalBlend);
      }

      if (_secondarySwatch != null)
      {
        mpb.SetColor(ShaderPropertyID._TC2Color, _secondarySwatch.Color);
        mpb.SetFloat(ShaderPropertyID._TC2Metalness, _secondarySwatch.Metalness);
        mpb.SetFloat(ShaderPropertyID._TC2Smoothness, _secondarySwatch.Smoothness);
        mpb.SetFloat(ShaderPropertyID._TC2SmoothBlend, _secondarySwatch.SmoothBlend);
        mpb.SetFloat(ShaderPropertyID._TC2MetalBlend, _secondarySwatch.MetalBlend);
      }

      _renderers[i].SetPropertyBlock(mpb);
    }
  }

  public void Apply()
  {
    Utils.Log($"[ColorZone] Applying swatches to materials", LogType.Editor);
    if (_materials != null)
    {
      for (int i = 0; i < _materials.Length; i++)
      {
        if (_primarySwatch != null)
        {
          _materials[i].SetColor(ShaderPropertyID._TC1Color, _primarySwatch.Color);
          _materials[i].SetFloat(ShaderPropertyID._TC1Metalness, _primarySwatch.Metalness);
          _materials[i].SetFloat(ShaderPropertyID._TC1Smoothness, _primarySwatch.Smoothness);
          _materials[i].SetFloat(ShaderPropertyID._TC1SmoothBlend, _primarySwatch.SmoothBlend);
          _materials[i].SetFloat(ShaderPropertyID._TC1MetalBlend, _primarySwatch.MetalBlend);
        }

        if (_secondarySwatch != null)
        {
          _materials[i].SetColor(ShaderPropertyID._TC2Color, _secondarySwatch.Color);
          _materials[i].SetFloat(ShaderPropertyID._TC2Metalness, _secondarySwatch.Metalness);
          _materials[i].SetFloat(ShaderPropertyID._TC2Smoothness, _secondarySwatch.Smoothness);
          _materials[i].SetFloat(ShaderPropertyID._TC2SmoothBlend, _secondarySwatch.SmoothBlend);
          _materials[i].SetFloat(ShaderPropertyID._TC2MetalBlend, _secondarySwatch.MetalBlend);
        }
      }
    }
  }
}
