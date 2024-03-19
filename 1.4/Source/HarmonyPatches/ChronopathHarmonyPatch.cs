using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MrSamuelStreamer.RPGAdventureFlavourPack;
using RimWorld;
using RimWorld.Planet;
using VanillaPsycastsExpanded;
using Verse;

namespace MSSRPG_VPE.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_AgeTracker), nameof(Pawn_AgeTracker.BiologicalTicksPerTick), MethodType.Getter)]
[HarmonyBefore("OskarPotocki.VanillaPsycastsExpanded")]
public static class ChronopathHarmonyPatch
{
    public static Dictionary<int, Tuple<int, int, float>> ChronopathsByMap = new(100);
    public static Dictionary<int, Tuple<int, int, float>> ChronopathsByCaravan = new(30);
    public static int LastChronopathUpdateTick = 0;
    public static int NextChronopathUpdateTick = 0;

    private static Lazy<PsycasterPathDef> ChronopathDef = new(() => DefDatabase<PsycasterPathDef>.GetNamedSilentFail("VPE_Chronopath"));

    [HarmonyPostfix]
    public static void Postfix(ref float __result, Pawn ___pawn)
    {
        if (Find.TickManager.TicksGame > NextChronopathUpdateTick)
        {
            NextChronopathUpdateTick = Find.TickManager.TicksGame + 35000;
            Task.Run(UpdateChronopathData);
        }

        if (___pawn.RaceProps.Humanlike && ___pawn.ageTracker.AgeBiologicalYears >= RPGAdventureFlavourPack.Settings.ElderAgeThreshold)
            __result *= RPGAdventureFlavourPack.Settings.ElderAgeMultiplier;

        if (ChronopathDef.Value == null ||
            GetNextTickForPawnWithCount(___pawn) is not { } lastTickForPawnWithCount ||
            !IsChronoPath(___pawn)) return;

        __result *= lastTickForPawnWithCount.Item3;
    }

    private static void UpdateChronopathData()
    {
        LastChronopathUpdateTick = Find.TickManager.TicksGame;

        try
        {
            ChronopathsByMap = Find.Maps.Select(m =>
            {
                List<Pawn> pawns = m.mapPawns?.AllPawns?.Where(IsChronoPath).ToList() ?? [];
                return new
                {
                    MapId = m.uniqueID, Value = new Tuple<int, int, float>(LastChronopathUpdateTick + 60000, pawns.Count, ChronopathAgeMultiplierForCount(pawns.Count))
                };
            }).ToDictionary(pair => pair.MapId, pair => pair.Value);

            ChronopathsByCaravan = Find.WorldObjects.Caravans.Select(c =>
            {
                List<Pawn> pawns = c.pawns?.InnerListForReading?.Where(IsChronoPath).ToList() ?? [];
                return new
                {
                    CaravanId = c.ID, Value = new Tuple<int, int, float>(LastChronopathUpdateTick + 60000, pawns.Count, ChronopathAgeMultiplierForCount(pawns.Count))
                };
            }).ToDictionary(pair => pair.CaravanId, pair => pair.Value);
        }
        catch (Exception e)
        {
            Log.Warning($"Error updating Chronopath data in RPGAdventureFlavourPack, won't try again for one game day, error was:\n{e}");
            NextChronopathUpdateTick = Find.TickManager.TicksGame + 60000;
        }
    }

    private static int NextTickOffset()
    {
        return 25000 + Rand.Range(500, 10000);
    }

    private static Tuple<int, int, float> GetNextTickForPawnWithCount(Pawn pawn, bool forceUpdate = false)
    {
        Tuple<int, int, float> tickToCountPair = null;
        if (pawn.MapHeld is { } map)
        {
            ChronopathsByMap.TryGetValue(map.uniqueID, out tickToCountPair);
            if (forceUpdate || (tickToCountPair?.Item1 ?? int.MaxValue) < Find.TickManager.TicksGame)
            {
                LastChronopathUpdateTick = Find.TickManager.TicksGame;
                int count = map.mapPawns?.AllPawns?.Count(IsChronoPath) ?? 0;
                tickToCountPair = new Tuple<int, int, float>(LastChronopathUpdateTick + NextTickOffset(), count, ChronopathAgeMultiplierForCount(count));
                ChronopathsByMap.SetOrAdd(map.uniqueID, tickToCountPair);
            }
        }
        else if (pawn.GetCaravan() is { } caravan)
        {
            ChronopathsByCaravan.TryGetValue(caravan.ID, out tickToCountPair);
            if (forceUpdate || (tickToCountPair?.Item1 ?? int.MaxValue) < Find.TickManager.TicksGame)
            {
                LastChronopathUpdateTick = Find.TickManager.TicksGame;
                int count = caravan.pawns?.InnerListForReading?.Count(IsChronoPath) ?? 0;
                tickToCountPair = new Tuple<int, int, float>(LastChronopathUpdateTick + NextTickOffset(), count, ChronopathAgeMultiplierForCount(count));
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
        Tuple<int, int, float> nextTickForPawnWithCount = GetNextTickForPawnWithCount(pawn, fastUpdate && Find.TickManager.TicksGame - LastChronopathUpdateTick > 600);
        chronopathCount = Math.Max(nextTickForPawnWithCount?.Item2 ?? 1, 1);
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
