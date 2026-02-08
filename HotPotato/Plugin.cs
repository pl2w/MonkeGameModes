using BepInEx;
using BepInEx.Logging;
using HotPotato.GameModes;
using Utilla.Attributes;
using MonkeLib.Assets;
using UnityEngine;

namespace HotPotato;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.25")]
[BepInDependency("xyz.pl2w.monkelib", "0.1.0")]
[ModdedGamemode(GameModeInfo.Guid, GameModeInfo.Name, typeof(HotPotatoManager))]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    public static Texture2D potatoTexture, burntPotatoTexture;

    public Plugin()
    {
        Log = Logger;
        Log.LogInfo($"Running on Gorilla Tag version: ({NetworkSystemConfig.AppVersion}).");
        potatoTexture = AssetLoading.LoadTextureFromEmbed("HotPotato.Assets.potato.png");
        burntPotatoTexture = AssetLoading.LoadTextureFromEmbed("HotPotato.Assets.burntpotato.png");
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w_chin.hotpotato";
    public const string Name = "Hot Potato";
    public const string Version = "0.1.0";
}

public static class GameModeInfo
{
    public const string Guid = "xyz.pl2w_chin.hotpotato";
    public const string Name = "HOT POTATO";
    public const int Id = 4822;
}