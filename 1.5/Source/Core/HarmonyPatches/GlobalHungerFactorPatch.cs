using HarmonyLib;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.HarmonyPatches;

[HarmonyPatch(typeof(HediffSet), nameof(HediffSet.GetHungerRateFactor))]
public static class GlobalHungerFactorPatch
{
    [HarmonyPostfix]
    public static void Postfix(ref float __result)
    {
        __result *= RPGAdventureFlavourPack.Settings.GetGlobalHungerFactor();
    }
}
