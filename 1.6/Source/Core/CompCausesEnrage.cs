using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class CompCausesEnrage : ThingComp
{
    public int lastUsedTick = -99999;
    public int lastCausedEnrageTick = -999999;

    public CompProperties_CausesEnrage Props => (CompProperties_CausesEnrage)props;

    public void Notify_UsedThisTick() => lastUsedTick = Find.TickManager.TicksGame;

    public bool UsedLastTick() => lastUsedTick >= Find.TickManager.TicksGame - 1;


    public bool CanCreateInfestationNow => UsedLastTick() && !CantFireBecauseCausedEnrageRecently && !CantFireBecauseSomethingElseCausedEnrageRecently;

    public bool CantFireBecauseCausedEnrageRecently => Find.TickManager.TicksGame <= lastCausedEnrageTick + 60000 * Props.minRefireDays;

    public bool CantFireBecauseSomethingElseCausedEnrageRecently => parent.Spawned && parent.Map.listerBuildingWithTagInProximity
        .GetForCell(parent.Position, Props.preventEnrageDist, "MSSRPG_CausesEnrage").Any(c => c.TryGetComp<CompCausesEnrage>()?.CantFireBecauseCausedEnrageRecently ?? false);

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref lastCausedEnrageTick, "lastCausedEnrageTick", -999999);
    }

    public void Notify_CausedEnrage()
    {
        lastCausedEnrageTick = Find.TickManager.TicksGame;
    }

    public bool GetEnragedPawns(IncidentParms incidentParms, out List<Pawn> pawns)
    {
        int num = 0;
        pawns = [];
        float pointsRemaining = incidentParms.points;
        while (pointsRemaining > 0)
        {
            if (num > 100)
            {
                Log.Error("Too many iterations.");
                break;
            }

            float remaining = pointsRemaining;
            Props.spawnablePawnKinds.Where(x => x.combatPower <= (double)remaining).TryRandomElement(out PawnKindDef pawnKindDef);
            if (pawnKindDef == null)
            {
                if (num == 0) pawnKindDef = Props.spawnablePawnKinds.RandomElement();
                else break;
            }

            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKindDef, forceGenerateNewPawn: true));
            if (pawn != null)
            {
                pawns.Add(pawn);
                pointsRemaining -= pawnKindDef.combatPower;
            }

            ++num;
        }

        return pawns.Any();
    }
}