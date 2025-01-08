using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Technicolor
{
  [Serializable]
  public class ColorZone : ScriptableObject
  {
    public TechnicolorSwatch PrimarySwatch => _primarySwatch;
    public TechnicolorSwatch SecondarySwatch => _secondarySwatch;

    [SerializeField]
    public string ZoneName = "main";
    [SerializeField]
    public string PrimarySwatchName = "";
    [SerializeField]
    public string SecondarySwatchName = "";
    [SerializeField]
    private string[] _transforms;

    private TechnicolorSwatch _primarySwatch;
    private TechnicolorSwatch _secondarySwatch;

    private Part _part;
    private Material[] materials;

    public ColorZone()
    { }
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
      _primarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(PrimarySwatchName);
      _secondarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(SecondarySwatchName);
      Apply();
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
        Renderer[] candidates = _part.GetComponentsInChildren<Renderer>(true);
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

      List<Material> mats = new();
      for (int i = 0; i < renderers.Count; i++)
      {
        if (renderers[i].material.shader.name == TechnicolorConstants.TEAMCOLOR_SHADER_NAME)
        {
          mats.Add(renderers[i].material);
        }
      }
      materials = mats.ToArray();
    }

    public void SetSwatch(string primaryName, string secondaryName)
    {
      PrimarySwatchName = primaryName;
      SecondarySwatchName = secondaryName;

      _primarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(primaryName);
      _secondarySwatch = TechnicolorData.Instance.SwatchLibrary.GetSwatch(secondaryName);
    }
    public void SetSwatch(TechnicolorSwatch primary, TechnicolorSwatch secondary)
    {
      PrimarySwatchName = primary.Name;
      SecondarySwatchName = secondary.Name;

      _primarySwatch = primary;
      _secondarySwatch = secondary;
    }
    public void Apply()
    {
      Utils.Log($"[ColorZone] Applying swatches to materials", LogType.Any);
      if (materials != null)
      {
        for (int i = 0; i < materials.Length; i++)
        {
          if (_primarySwatch != null)
          {
            materials[i].SetColor(ShaderPropertyID._TC1Color, _primarySwatch.Color);
            materials[i].SetFloat(ShaderPropertyID._TC1Metalness, _primarySwatch.Metalness);
            materials[i].SetFloat(ShaderPropertyID._TC1Smoothness, _primarySwatch.Smoothness);
            materials[i].SetFloat(ShaderPropertyID._TC1SmoothBlend, _primarySwatch.SmoothBlend);
            materials[i].SetFloat(ShaderPropertyID._TC1MetalBlend, _primarySwatch.MetalBlend);
          }
          if (_secondarySwatch != null)
          {
            materials[i].SetColor(ShaderPropertyID._TC2Color, _secondarySwatch.Color);
            materials[i].SetFloat(ShaderPropertyID._TC2Metalness, _secondarySwatch.Metalness);
            materials[i].SetFloat(ShaderPropertyID._TC2Smoothness, _secondarySwatch.Smoothness);
            materials[i].SetFloat(ShaderPropertyID._TC2SmoothBlend, _secondarySwatch.SmoothBlend);
            materials[i].SetFloat(ShaderPropertyID._TC2MetalBlend, _secondarySwatch.MetalBlend);
          }
        }
      }
    }
  }
}
