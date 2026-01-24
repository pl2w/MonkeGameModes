using System;

namespace MonkeUtils;

[BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
[ModdedGamemode(GameModeInfo.TeamTagGuid, GameModeInfo.TeamTagName, typeof(TeamTag))]
public class Plugin : BaseUnityPlugin
{
    public Plugin()
    {
        var harmony = new Harmony(PluginInfo.Guid);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}

public static class PluginInfo
{
    public const string Guid = "xyz.pl2w.gtag.partygames";
    public const string Name = "Gorilla Tag Party Games";
    public const string Version = "0.1.0";
}