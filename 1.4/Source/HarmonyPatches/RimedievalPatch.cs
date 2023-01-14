using HarmonyLib;
using Rimedieval;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.Rimedieval.HarmonyPatches
{
    [HarmonyPatch(typeof(DefCleaner), "IsAllowedForRimedieval")]
    public static class IsAllowedForRimedievalPatches
    {
        [HarmonyPrefix]
        public static bool IsAllowedForRimedievalPrefix(this ThingDef thingDef, ref bool __result)
        {
            if (thingDef?.defName != "Genepack") return true;
            __result = true;
            return false;
        }
    }
}
