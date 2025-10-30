using HarmonyLib;
using UnityEngine;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack
{
    public class RPGAdventureFlavourPack : Mod
    {
        public static RPGAdventureFlavourPackSettings Settings;

        public RPGAdventureFlavourPack(ModContentPack content) : base(content)
        {
            Settings = GetSettings<RPGAdventureFlavourPackSettings>();

            new Harmony("mrsamuelstreamer.rpgadventureflavourpack.core").PatchAll();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Settings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "RPGAdventureFlavourPackSettings_Name".TranslateSimple();
        }
    }
}
