using HarmonyLib;
using Reptile.Phone;

namespace BeGayDoCrime.Patches;

[HarmonyPatch(typeof(AppGraffiti))]
public class AppGraffitiPatch {
    [HarmonyPostfix]
    [HarmonyPatch("RefreshList")]
    private static void RefreshList(AppGraffiti __instance) {
        __instance.GraffitiArt.ForEach(Plugin.ApplyCustomGraffiti);
    }
}
