using RimWorld;
using RimWorld.Planet;
using System;
using Verse;
using VFECore;
using VFECore.Abilities;

namespace VPE_Ranger
{
    public class Ability_RicochetShot : VFECore.Abilities.Ability
    {
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (!pawn.equipment.Primary.def.IsRangedWeapon)
            {
                Messages.Message("main weapon must be ranged weapon", MessageTypeDefOf.NeutralEvent, false);
                return false;
            }
            return base.ValidateTarget(target, showMessages);
        }
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            Projectile projectile = (Projectile)GenSpawn.Spawn(def.GetModExtension<AbilityExtension_Projectile>().projectile, pawn.Position, pawn.Map);
            projectile.Launch(pawn, targets[0].Thing, targets[0].Thing, ProjectileHitFlags.IntendedTarget);
        }
    }
}
