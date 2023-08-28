using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using Reptile.Phone;
using UnityEngine;

namespace BeGayDoCrime;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Bomb Rush Cyberfunk.exe")]
public class Plugin : BaseUnityPlugin {
    public static string CustomGraffitiFolder = null!;
    public static ManualLogSource Log = null!;
    public static Harmony Harmony = null!;

    // Account for custom graffiti names being written to the save file
    public static Dictionary<string, string> CustomToVanillaGraffiti = new();

    private void Awake() {
        Log = this.Logger;

        CustomGraffitiFolder = Path.Combine(Paths.ConfigPath, "BeGayDoCrime");
        if (!Directory.Exists(CustomGraffitiFolder)) {
            Directory.CreateDirectory(CustomGraffitiFolder);
        }

        var textFiles = Directory.GetFiles(CustomGraffitiFolder, "*.txt");
        foreach (var textFile in textFiles) {
            var origName = Path.GetFileNameWithoutExtension(textFile);
            var metadata = GetMetadataFromArt(origName);
            if (metadata is not null) {
                var newName = metadata[0];
                Log.LogInfo($"Found custom graffiti: {origName} -> {newName}");
                CustomToVanillaGraffiti[newName] = origName;
            }
        }

        this.SetupHarmony();
    }

    private void SetupHarmony() {
        Harmony = new Harmony("BeGayDoCrime.Harmony");

        var patches = typeof(Plugin).Assembly.GetTypes()
                                    .Where(m => m.GetCustomAttributes(typeof(HarmonyPatch), false).Length > 0)
                                    .ToArray();

        foreach (var patch in patches) {
            Harmony.PatchAll(patch);
        }
    }

    private static Texture2D? GetTextureFromArt(string name, int width, int height) {
        var path = Path.Combine(CustomGraffitiFolder, name + ".png");

        if (File.Exists(path)) {
            var bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(width, height);
            texture.LoadImage(bytes);
            return texture;
        }

        return null;
    }

    // no tuples in this net version ig lol
    private static List<string>? GetMetadataFromArt(string name) {
        var path = Path.Combine(CustomGraffitiFolder, name + ".txt");

        if (File.Exists(path)) {
            var lines = File.ReadAllLines(path);
            if (lines.Length >= 2) {
                return new List<string> {lines[0], lines[1]};
            }
        }

        return null;
    }

    public static void ApplyCustomGraffiti(GraffitiArt? art) {
        if (art is null) return;

        var title = CustomToVanillaGraffiti.TryGetValue(art.title, out var vanillaTitle) ? vanillaTitle : art.title;

        var oldTexture = art.graffitiMaterial.mainTexture;
        var newTexture = GetTextureFromArt(title, oldTexture.width, oldTexture.height);
        if (newTexture is not null) {
            art.graffitiMaterial.mainTexture = newTexture;
        }

        var metadata = GetMetadataFromArt(title);
        if (metadata is not null) {
            art.title = metadata[0];
            art.artistName = metadata[1];
        }
    }

    public static void ApplyCustomGraffiti(GraffitiAppEntry? entry) {
        if (entry is null) return;

        var title = CustomToVanillaGraffiti.TryGetValue(entry.Title, out var vanillaTitle) ? vanillaTitle : entry.Title;

        var oldTexture = entry.GraffitiTexture;
        var newTexture = GetTextureFromArt(title, oldTexture.width, oldTexture.height);
        if (newTexture is not null) {
            entry.GraffitiTexture = newTexture;
        }

        var metadata = GetMetadataFromArt(title);
        if (metadata is not null) {
            entry.Title = metadata[0];
            entry.Artist = metadata[1];
        }
    }

    // Have to do this because it wrote to the save file
    // ...and I only noticed after I published it
    public static string? RepairName(string name) {
        return CustomToVanillaGraffiti.TryGetValue(name, out var vanillaName) ? vanillaName : null;
    }
}
