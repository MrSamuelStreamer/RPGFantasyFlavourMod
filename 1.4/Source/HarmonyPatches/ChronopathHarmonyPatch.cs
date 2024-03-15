using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MrSamuelStreamer.RPGAdventureFlavourPack;
using MSSRPG_VPE.HarmonyPatches;
using RimWorld;
using RimWorld.Planet;
using VanillaPsycastsExpanded;
using Verse;

namespace MSSRPG_VPE.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.BiologicalTicksPerTick), MethodType.Getter)]
[HarmonyBefore("OskarPotocki.VanillaPsycastsExpanded")]
public static class ChronopathHarmonyPatch
{
    public static Dictionary<int, Pair<int, int>?> ChronopathsByMap = new(100);
    public static Dictionary<int, Pair<int, int>?> ChronopathsByCaravan = new(30);
    public static int LastChronopathUpdateTick = 0;

    private static Lazy<PsycasterPathDef> ChronopathDef = new(() => DefDatabase<PsycasterPathDef>.GetNamedSilentFail("VPE_Chronopath"));

    [HarmonyPostfix]
    public static void Postfix(ref float __result, Pawn ___pawn)
    {
        if (ChronopathDef.Value == null ||
            GetNextTickForPawnWithCount(___pawn) is not { } lastTickForPawnWithCount ||
            !IsChronoPath(___pawn)) return;

        __result *= ChronopathAgeMultiplierForCount(lastTickForPawnWithCount.Second);
    }

    private static int NextTickOffset()
    {
        return 25000 + Rand.Range(500, 10000);
    }

    private static Pair<int, int>? GetNextTickForPawnWithCount(Pawn pawn, bool forceUpdate = false)
    {
        Pair<int, int>? tickToCountPair = null;
        if (pawn.MapHeld is {} map)
        {
            ChronopathsByMap.TryGetValue(map.uniqueID, out tickToCountPair);
            if (forceUpdate || (tickToCountPair?.First ?? 0) < Find.TickManager.TicksGame)
            {
                LastChronopathUpdateTick = Find.TickManager.TicksGame;
                tickToCountPair = new Pair<int, int>(LastChronopathUpdateTick + NextTickOffset(), map.mapPawns?.AllPawns?.Count(IsChronoPath) ?? 0);
                ChronopathsByMap.SetOrAdd(map.uniqueID, tickToCountPair);
            }
        }
        else if (pawn.GetCaravan() is { } caravan)
        {
            ChronopathsByCaravan.TryGetValue(caravan.ID, out tickToCountPair);
            if (forceUpdate || (tickToCountPair?.First ?? 0) < Find.TickManager.TicksGame)
            {
                LastChronopathUpdateTick = Find.TickManager.TicksGame;
                tickToCountPair = new Pair<int, int>(LastChronopathUpdateTick + NextTickOffset(), caravan.pawns?.InnerListForReading?.Count(IsChronoPath) ?? 0);
                ChronopathsByCaravan.SetOrAdd(caravan.ID, tickToCountPair);
            }
        }

        return tickToCountPair;
    }

    public static bool IsChronoPath(Pawn pawn) =>
        !pawn.health.Dead &&
        pawn.RaceProps.Humanlike &&
        pawn.health.hediffSet.GetFirstHediffOfDef(VPE_DefOf.VPE_PsycastAbilityImplant) is Hediff_PsycastAbilities psycastAbilities &&
        psycastAbilities.unlockedPaths.Contains(ChronopathDef.Value);

    public static float ChronopathAgeMultiplierForCount(int chronopathsOnMap)
    {
        return RPGAdventureFlavourPack.Settings.GetChronoFieldAgeCurve().Evaluate(chronopathsOnMap);
    }

    public static float ChronopathAgeMultiplier(Pawn pawn, out int chronopathCount, bool fastUpdate = false)
    {
        Pair<int,int>? nextTickForPawnWithCount = GetNextTickForPawnWithCount(pawn, fastUpdate && Find.TickManager.TicksGame - LastChronopathUpdateTick > 600);
        chronopathCount = Math.Max(nextTickForPawnWithCount?.Second ?? 1, 1);
        return ChronopathAgeMultiplierForCount(chronopathCount);
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

        float ageMult = ChronopathHarmonyPatch.ChronopathAgeMultiplier(__instance, out int chronopathCount, true);
        yield return new StatDrawEntry(StatCategoryDefOf.BasicsPawn, "RPGAdventureFlavourPackSettings_StatsReport_ChronoRateMultiplier".Translate(),
            ageMult.ToStringPercent(), "RPGAdventureFlavourPackSettings_StatsReport_ChronoRateMultiplier_Desc".Translate(chronopathCount, ageMult), 4196);
    }
}
