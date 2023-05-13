using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace VPE_Ranger
{
    public class Ability_RicochetShot : Ability
    {
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (pawn.equipment?.Primary?.def?.IsRangedWeapon ?? false)
                return base.ValidateTarget(target, showMessages);
            Messages.Message("Main weapon must be ranged weapon", MessageTypeDefOf.NeutralEvent, false);
            return false;
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            Projectile projectile =
                (Projectile)GenSpawn.Spawn(def.GetModExtension<AbilityExtension_Projectile>().projectile, pawn.Position,
                    pawn.Map);
            projectile.Launch(pawn, targets[0].Thing, targets[0].Thing, ProjectileHitFlags.IntendedTarget);
        }
    }
}
