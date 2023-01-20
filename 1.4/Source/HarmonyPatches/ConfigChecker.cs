using HarmonyLib;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.HarmonyPatches;

[HarmonyPatch(typeof(GameComponentUtility), nameof(GameComponentUtility.FinalizeInit))]
public static class ConfigChecker
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!RPGAdventureFlavourPack.Settings.ConfigsApplied)
            Find.LetterStack.ReceiveLetter("RPGAdventureFlavourPack_Config_Warning_Label".Translate(),
                "RPGAdventureFlavourPack_Config_Warning_Message".Translate(), LetterDefOf.NegativeEvent);
    }
}
