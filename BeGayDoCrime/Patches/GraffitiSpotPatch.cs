using HarmonyLib;
using Reptile;

namespace BeGayDoCrime.Patches; 

[HarmonyPatch(typeof(GraffitiSpot))]
public class GraffitiSpotPatch {
    [HarmonyPrefix]
    [HarmonyPatch("InitializeGraffiti")]
    internal static void InitializeGraffiti(GraffitiArt topGraffitiArt, GraffitiArt bottomGraffitiArt) {
        Plugin.ApplyCustomGraffiti(topGraffitiArt);
        Plugin.ApplyCustomGraffiti(bottomGraffitiArt);
    }

    [HarmonyPrefix]
    [HarmonyPatch("Paint")]
    internal static void Paint(Crew newCrew, GraffitiArt graffitiArt, Player byPlayer = null) {
        Plugin.ApplyCustomGraffiti(graffitiArt);
    }
}
