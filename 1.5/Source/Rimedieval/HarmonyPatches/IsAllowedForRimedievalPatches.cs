using System.Collections.Generic;
using HarmonyLib;
using Rimedieval;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.Rimedieval.HarmonyPatches
{
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(DefCleaner), "IsAllowedForRimedieval")]
    public static class IsAllowedForRimedievalPatches
    {
        private static HashSet<string> _defsToAllow = [];
        private static bool _disableRimMedievalBypass;

        static IsAllowedForRimedievalPatches()
        {
            RPGAdventureFlavourPack.Settings.RegisterSettingsUpdatedAction(UpdateDefs);
            UpdateDefs();
        }

        private static void UpdateDefs()
        {
            _defsToAllow.Clear();
            _defsToAllow.AddRange(RPGAdventureFlavourPack.Settings.ExtraMedievalDefs());
            _disableRimMedievalBypass = _defsToAllow.Count == 0;
        }

        [HarmonyPrefix]
        public static bool IsAllowedForRimedievalPrefix(this ThingDef thingDef, ref bool __result)
        {
            if (_disableRimMedievalBypass || (thingDef != null && !_defsToAllow.Contains(thingDef.defName))) return true;
            __result = true;
            return false;
        }
    }
}
