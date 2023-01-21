using RimWorld;
using Verse;

namespace VPE_Ranger
{
    public class HediffComp_DoubleTap : HediffComp
    {
        public HediffCompProperties_DoubleTap Props => (HediffCompProperties_DoubleTap)props;

        public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
        {
            base.Notify_PawnUsedVerb(verb, target);
            if (!Pawn.equipment.Primary.def.IsRangedWeapon || verb.IsMeleeAttack)
            {
                return;
            }

            Projectile projectile =
                (Projectile)GenSpawn.Spawn(verb.GetProjectile() ?? Props.projectile, Pawn.Position, Pawn.Map);
            float rand = Rand.Value;
            if (rand <= Pawn.GetStatValue(StatDefOf.ShootingAccuracyPawn))
            {
                projectile.Launch(Pawn, target, target, ProjectileHitFlags.IntendedTarget);
            }
            else
            {
                projectile.Launch(Pawn, target.Thing.Position.RandomAdjacentCell8Way(), target,
                    ProjectileHitFlags.IntendedTarget);
            }
        }
    }
}
