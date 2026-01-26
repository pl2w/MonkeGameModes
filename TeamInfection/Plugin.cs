using BepInEx;
using BepInEx.Logging;
using TeamInfection.GameModes;
using Utilla.Attributes;

namespace TeamInfection;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.25")]
[ModdedGamemode(GameModeInfo.TeamInfectionGuid, GameModeInfo.TeamInfectionName, typeof(TeamInfectionManager))]
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
    public const string Guid = "xyz.pl2w.gtag.partygames.teaminfection";
    public const string Name = "Team Infection";
    public const string Version = "0.1.0";
}

public static class GameModeInfo
{
    public const string TeamInfectionGuid = "xyz.pl2w.gtag.partygames.teaminfection";
    public const string TeamInfectionName = "TEAM INFECTION";
    public const int TeamInfectionId = 4821;
}