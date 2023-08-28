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

    [HarmonyPrefix]
    [HarmonyPatch("ReadFromData")]
    public static void ReadFromData(GraffitiSpot __instance) {
        var progressableData = Traverse.Create(__instance).Field<GraffitiSpotProgress>("progressableData").Value;

        var newTop = Plugin.RepairName(progressableData.topGraffitiArt);
        if (newTop != null) progressableData.topGraffitiArt = newTop;

        var newBottom = Plugin.RepairName(progressableData.bottomGraffitiArt);
        if (newBottom != null) progressableData.bottomGraffitiArt = newBottom;
    }
}
