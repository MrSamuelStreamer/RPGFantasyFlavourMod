using HarmonyLib;
using RimQuest;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.RimQuest
{
    public class RPGAdventureFlavourPackForRimQuest : Mod
    {
        public RPGAdventureFlavourPackForRimQuest(ModContentPack content) : base(content)
        {
            new Harmony("mrsamuelstreamer.rpgadventureflavourpack.rimquest").PatchAll();
        }
    }

    [StaticConstructorOnStartup]
    public static class PreloadQuests
    {
        static PreloadQuests()
        {
            Log.Message("Adding Witcher Monsters to RimQuest");
            Main.UpdateValidQuests(true);
        }
    }
}
