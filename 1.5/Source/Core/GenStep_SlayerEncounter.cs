using RimWorld;
using Verse;

namespace MrSamuelStreamer.RPGAdventureFlavourPack;

public class GenStep_SlayerEncounter : GenStep
{
    public override void Generate(Map map, GenStepParams parms)
    {
        TraverseParms traverseParams = TraverseParms.For(TraverseMode.PassAllDestroyableThings);
        if (!RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith(
                cell =>
                    cell.Standable(map) && map.reachability.CanReachMapEdge(cell, traverseParams) &&
                    cell.GetRoom(map).CellCount >= 10, map, out IntVec3 result))
            return;

        PawnKindDef monsterKind = parms.sitePart?.parms?.animalKind ?? def
            .GetModExtension<CreatureSelectionExtension>().possiblePawnKinds
            .RandomElementByWeight(c => c.RaceProps?.wildness ?? 0.1f);

        Pawn target = PawnGenerator.GeneratePawn(new PawnGenerationRequest(monsterKind, tile: map.Tile));
        IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(result, map, 10);
        GenSpawn.Spawn(target, loc, map, Rot4.Random);
        target.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent);
    }

    public override int SeedPart => 3432781;
}
