namespace Technicolor;

public class ProceduralFairingConfig
{

  [Persistent] public readonly string FairingAlbedoTexture;
  [Persistent] public readonly string FairingMetalTexture;
  [Persistent] public readonly string FairingNormalTexture;
  [Persistent] public readonly string FairingTCTexture;

  [Persistent] public readonly string CapAlbedoTexture;
  [Persistent] public readonly string CapMetalTexture;
  [Persistent] public readonly string CapNormalTexture;
  [Persistent] public readonly string CapTCTexture;

  public ProceduralFairingConfig() { }
  public ProceduralFairingConfig(ConfigNode node)
  {
    ConfigNode.LoadObjectFromConfig(this, node);
  }
}

