using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class IncidentWorker_TriggerEnrage : IncidentWorker
{
    public static List<Building> GetEnrageCausers(Map map)
    {
        return map.listerBuildings.allBuildingsColonist
            .Where(b => ThingCompUtility.TryGetComp<CompCausesEnrage>(b) is { CanCreateInfestationNow: true }).ToList();
    }

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!base.CanFireNowSub(parms)) return false;
        Map target = (Map)parms.target;
        return GetEnrageCausers(target).Any();
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        List<Building> tmpCause = GetEnrageCausers(map);
        if (!tmpCause.TryRandomElement(out Thing cause))
            return false;
        CompCausesEnrage compCausesEnrage = cause.TryGetComp<CompCausesEnrage>();
        if (!(compCausesEnrage?.GetEnragedPawns(parms, out List<Pawn> enragedPawns) ?? false)) return false;

        Pawn firstSpawned = null;
        foreach (Pawn enragedPawn in enragedPawns)
        {
            if (enragedPawn == null) continue;
            CellFinder.TryFindRandomSpawnCellForPawnNear(cause.Position, map, out IntVec3 wipeSpawnLocNear,
                extraValidator: x => x.Walkable(map) && x.GetFirstThing(map, cause.def) == null && x.GetFirstThingWithComp<CompCausesEnrage>(map) == null);
            if (wipeSpawnLocNear == cause.Position || !wipeSpawnLocNear.IsValid) enragedPawn.Destroy();
            GenSpawn.Spawn(enragedPawn, wipeSpawnLocNear, map, WipeMode.FullRefund);

            enragedPawn.health?.AddHediff(HediffDefOf.Scaria);
            enragedPawn.mindState.mentalStateHandler?.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
            enragedPawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.Range(60000, 120000);

            firstSpawned ??= enragedPawn;
        }

        if (firstSpawned == null) return false;
        compCausesEnrage.Notify_CausedEnrage();
        compCausesEnrage.parent.TryGetComp<CompCreatesInfestations>()?.Notify_CreatedInfestation();
        SendStandardLetter(parms, new TargetInfo(firstSpawned.Position, map));
        return true;
    }
}