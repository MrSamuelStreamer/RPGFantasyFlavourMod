using RimWorld;
using RimWorld.Planet;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace VPE_Ranger
{
    public class Ability_SplitShot : Ability
    {
        public IntVec3 targetCell;

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (pawn.equipment?.Primary?.def?.IsRangedWeapon ?? false)
                return base.ValidateTarget(target, showMessages);
            Messages.Message("Main weapon must be ranged weapon", MessageTypeDefOf.NeutralEvent, false);
            return false;
        }

        public override void ModifyTargets(ref GlobalTargetInfo[] targets)
        {
            targetCell = targets[0].Cell;
            base.ModifyTargets(ref targets);
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var count = 0;
            foreach (GlobalTargetInfo target in targets)
            {
                if (!(target.Thing is Pawn targetPawn)) continue;

                if (targetPawn.HostileTo(pawn)
                    || (targetPawn.Faction?.HostileTo(pawn.Faction) ?? true)
                    || ((targetPawn.RaceProps?.Animal ?? false) &&
                        (targetPawn.mindState?.mentalStateHandler?.CurState?.def == MentalStateDefOf.Manhunter ||
                         targetPawn.mindState?.mentalStateHandler?.CurState?.def == MentalStateDefOf.Berserk)))
                {
                    Projectile projectile =
                        (Projectile)GenSpawn.Spawn(pawn.equipment.PrimaryEq.PrimaryVerb.GetProjectile(), pawn.Position,
                            pawn.Map);
                    projectile.Launch(pawn, targetPawn, targetPawn, ProjectileHitFlags.IntendedTarget,
                        equipment: pawn.equipment.Primary);
                    count++;
                }

                if (count >= 5) break;
            }
        }
    }
}
