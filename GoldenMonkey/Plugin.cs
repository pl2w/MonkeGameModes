using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using GoldenMonkey.GameModes;
using HarmonyLib;
using UnityEngine;
using Utilla.Attributes;

namespace GoldenMonkey;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.25")]
[BepInDependency("xyz.pl2w.monkelib", "0.1.0")]
[ModdedGamemode(GameModeInfo.Guid, GameModeInfo.Name, typeof(GoldenMonkeyManager))]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource Log;
    
    public Plugin()
    {
        Log = Logger;
        Log.LogInfo($"Running on Gorilla Tag version: ({NetworkSystemConfig.AppVersion}).");

        var harmony = new Harmony(PluginInfo.Guid);
        harmony.PatchAll(Assembly.GetExecutingAssembly());   
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w_chin.goldenmonkey";
    public const string Name = "Golden Monkey";
    public const string Version = "0.1.0";
}

public static class GameModeInfo
{
    public const string Guid = "xyz.pl2w_chin.goldenmonkey";
    public const string Name = "GOLDEN MONKEY";
    public const int Id = 4823;
}