using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack.HarmonyPatches;

[HarmonyPatch(typeof(GenStep_AncientAltar), nameof(GenStep_AncientAltar.Generate))]
public static class Genstep_AncientAlterPatch
{
    [HarmonyPostfix]
    public static void Postfix(Map map, GenStepParams parms)
    {
        if (!RPGAdventureFlavourPack.Settings.DragonsInRelicSites) return;
        PawnKindDef pawnKindDef = DefDatabase<PawnKindDef>.AllDefsListForReading
            .Where(d => d.defName.Contains("Dragon") || d.defName.Contains("Wyvern"))
            .RandomElementWithFallback(PawnKindDefOf.Thrumbo);

        Pawn target = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnKindDef, tile: map.Tile));
        IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(map.Center, map, 10);
        GenSpawn.Spawn(target, loc, map, Rot4.Random);
        target.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
    }
}
