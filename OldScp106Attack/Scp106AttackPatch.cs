using CustomPlayerEffects;
using HarmonyLib;
using PlayerRoles.PlayableScps.Scp106;
using PlayerStatsSystem;
using PluginAPI.Events;

namespace OldScp106Attack;

[HarmonyPatch(typeof(Scp106Attack), nameof(Scp106Attack.ServerShoot))]
public static class Scp106AttackPatch
{
    [HarmonyPrefix]
    private static bool OnServerShoot(Scp106Attack __instance)
    {
        if (!__instance.VerifyShot())
            return false;
        
        PlayerEffectsController effectsController = __instance._targetHub.playerEffectsController;
        
        if (effectsController.GetEffect<Traumatized>().IsEnabled)
        {
            if (!__instance._targetHub.playerStats.DealDamage(new ScpDamageHandler(__instance.Owner, -1f, DeathTranslations.PocketDecay)))
                return false;
            __instance.VigorAmount += Scp106Attack.VigorCaptureReward;
            __instance.SendCooldown(__instance._hitCooldown);
            __instance.ReduceSinkholeCooldown();
            Hitmarker.SendHitmarkerDirectly(__instance.Owner, 1f);
        }
        else
        {
            if (!EventManager.ExecuteEvent(new Scp106TeleportPlayerEvent(__instance.Owner, __instance._targetHub)))
                return false;
            
            effectsController.EnableEffect<PocketCorroding>();
            __instance.VigorAmount += Scp106Attack.VigorCaptureReward;
            
            __instance.SendCooldown(__instance._hitCooldown);
            __instance.ReduceSinkholeCooldown();
            Hitmarker.SendHitmarkerDirectly(__instance.Owner, 1f);
        }
        
        return false;
    }
}