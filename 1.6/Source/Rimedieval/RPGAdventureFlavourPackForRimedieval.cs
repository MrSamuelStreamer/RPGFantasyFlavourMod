using HarmonyLib;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.Rimedieval
{
    [StaticConstructorOnStartup]
    public static class ApplyPatchesPostStartup
    {
        static ApplyPatchesPostStartup()
        {
            new Harmony("mrsamuelstreamer.rpgadventureflavourpack.rimedieval").PatchAll();
        }
    }

}
