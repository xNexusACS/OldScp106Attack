using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;

namespace OldScp106Attack;

public class EntryPoint
{
    private readonly Harmony _harmony = new("xnexusacs.oldlarryattack");
    
    [PluginConfig] 
    public Config Config;

    [PluginEntryPoint("OldScp106Attack", "1.0.0.0", "Brings back the old SCP-106 attack method", "xNexusACS")]
    private void Load()
    {
        if (!Config.Enabled)
        {
            Log.Info("The plugin is disabled in the config, aborting plugin startup");
            return;
        }
        
        _harmony.PatchAll();
    }

    [PluginUnload]
    private void UnLoad() => _harmony.UnpatchAll(_harmony.Id);
}