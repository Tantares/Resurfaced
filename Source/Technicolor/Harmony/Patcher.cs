using HarmonyLib;
using UnityEngine;

namespace Technicolor.HarmonyPatches;

[KSPAddon(KSPAddon.Startup.Instantly, true)]
public class TechnicolorHarmonyPatcher : MonoBehaviour
{
  public void Start()
  {
    Utils.Log("[Harmony] Start Patching");
    var harmony = new Harmony("Technicolor");
    harmony.PatchAll();
    Utils.Log("[Harmony] Patching complete");
  }
}

