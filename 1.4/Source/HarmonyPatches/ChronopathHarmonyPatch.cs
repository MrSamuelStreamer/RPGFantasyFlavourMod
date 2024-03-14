using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MrSamuelStreamer.RPGAdventureFlavourPack;
using MSSRPG_VPE.HarmonyPatches;
using RimWorld;
using VanillaPsycastsExpanded;
using Verse;

namespace MSSRPG_VPE.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.BiologicalTicksPerTick), MethodType.Getter)]
[HarmonyBefore("OskarPotocki.VanillaPsycastsExpanded")]
public static class ChronopathHarmonyPatch
{
    public static Map map = null;
    public static int ChronopathsOnMap = 0;
    public static int NextChronopathUpdateTick = 0;
    public static int LastChronopathUpdateTick = 0;

    private static Lazy<PsycasterPathDef> ChronopathDef = new(() => DefDatabase<PsycasterPathDef>.GetNamedSilentFail("VPE_Chronopath"));

    [HarmonyPostfix]
    public static void Postfix(ref float __result, Pawn ___pawn)
    {
        if (ChronopathDef.Value == null ||
            (___pawn.MapHeld == map && Find.TickManager.TicksGame < NextChronopathUpdateTick && ChronopathsOnMap == 0) ||
            !IsChronoPath(___pawn)) return;

        if (___pawn.MapHeld != null && (___pawn.MapHeld != map || Find.TickManager.TicksGame > NextChronopathUpdateTick))
        {
            UpdateChronopaths(___pawn);
        }

        if (ChronopathsOnMap == 0) return;
        __result *= ChronopathAgeMultiplier(___pawn);
    }

    private static void UpdateChronopaths(Pawn pawn)
    {
        map = pawn.MapHeld;
        NextChronopathUpdateTick += 30000;
        LastChronopathUpdateTick = Find.TickManager.TicksGame;
        ChronopathsOnMap = map?.mapPawns?.AllPawns?.Count(IsChronoPath) ?? 1;
    }

    public static bool IsChronoPath(Pawn pawn) =>
        !pawn.health.Dead &&
        pawn.health.hediffSet.GetFirstHediffOfDef(VPE_DefOf.VPE_PsycastAbilityImplant) is Hediff_PsycastAbilities psycastAbilities &&
        psycastAbilities.unlockedPaths.Contains(ChronopathDef.Value);

    public static float ChronopathAgeMultiplier(Pawn pawn, bool fastUpdate = false)
    {
        if (pawn.MapHeld != map || (fastUpdate && Find.TickManager.TicksGame - LastChronopathUpdateTick > 600)) UpdateChronopaths(pawn);
        return RPGAdventureFlavourPack.Settings.GetChronoFieldAgeCurve().Evaluate(ChronopathsOnMap);
    }
}

[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpecialDisplayStats))]
[HarmonyAfter("OskarPotocki.VanillaPsycastsExpanded")]
public static class ChronopathHarmonyPatchStats
{
    [HarmonyPostfix]
    public static IEnumerable<StatDrawEntry> Postfix(IEnumerable<StatDrawEntry> __result, Pawn __instance)
    {
        foreach (StatDrawEntry entry in __result) yield return entry;
        if (!ChronopathHarmonyPatch.IsChronoPath(__instance)) yield break;

        float ageMult = ChronopathHarmonyPatch.ChronopathAgeMultiplier(__instance, true);
        yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "RPGAdventureFlavourPackSettings_StatsReport_ChronoRateMultiplier".Translate(),
            ageMult.ToStringPercent(), "RPGAdventureFlavourPackSettings_StatsReport_ChronoRateMultiplier_Desc".Translate(ChronopathHarmonyPatch.ChronopathsOnMap, ageMult), 4196);
    }
}
