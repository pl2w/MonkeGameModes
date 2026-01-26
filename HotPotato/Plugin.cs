using BepInEx;
using BepInEx.Logging;
using HotPotato.GameModes;
using Utilla.Attributes;

namespace HotPotato;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.25")]
[ModdedGamemode(GameModeInfo.HotPotatoGuid, GameModeInfo.HotPotatoName, typeof(HotPotatoManager))]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;
    
    public Plugin()
    {
        Log = Logger;
        Log.LogInfo($"Running on Gorilla Tag version: ({NetworkSystemConfig.AppVersion}).");
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.gtag.partygames.hotpotato";
    public const string Name = "Hot Potato";
    public const string Version = "0.1.0";
}

public static class GameModeInfo
{
    public const string HotPotatoGuid = "xyz.pl2w.gtag.partygames.hotpotato";
    public const string HotPotatoName = "HOT POTATO";
    public const int HotPotatoId = 4822;
}