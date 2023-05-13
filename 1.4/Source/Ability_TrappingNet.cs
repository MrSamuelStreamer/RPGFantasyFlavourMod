using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace VPE_Ranger
{
    public class Ability_TrappingNet : Ability
    {
        public IntVec3 nearestTree;

        public bool treeCheck = false;

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!(pawn.equipment?.Primary?.def?.IsRangedWeapon ?? false))
            {
                Messages.Message("main weapon must be ranged weapon", MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            if (GenRadial.RadialCellsAround(target.Cell, 5f, true)
                .Any(cell => cell.GetFirstThing<Plant>(pawn.Map) != null || cell.GetFirstBuilding(pawn.Map) != null)) return true;

            if (!treeCheck) Messages.Message("no tree or wall nearby", MessageTypeDefOf.NeutralEvent, false);

            return false;
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            if (!(targets[0].Thing is Pawn targetPawn)) return;
            foreach (IntVec3 cell in GenRadial.RadialCellsAround(targets[0].Cell, GetRadiusForPawn(), true))
            {
                Plant plant = cell.GetFirstThing<Plant>(pawn.Map);
                if (plant != null && plant.def.ingestible.foodType == FoodTypeFlags.Tree)
                {
                    if (plant.LifeStage == PlantLifeStage.Mature || plant.LifeStage == PlantLifeStage.Growing)
                        nearestTree = cell;
                    break;
                }
                else if (cell.GetFirstBuilding(pawn.Map) != null)
                {
                    nearestTree = cell;
                    break;
                }
            }

            targetPawn.Position = nearestTree;
            Projectile projectile =
                (Projectile)GenSpawn.Spawn(def.GetModExtension<AbilityExtension_Projectile>().projectile,
                    pawn.Position, pawn.Map);
            projectile.Launch(pawn, nearestTree, nearestTree, ProjectileHitFlags.IntendedTarget);
        }
    }
}
