using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace VPE_Ranger
{
    public class Ability_ArrowRain : Ability
    {
        public IntVec3 targetCell;

        public int shotLeft;

        public int tickUntilNextShot;

        public int delay;

        private GlobalTargetInfo targetInfo;

        public override void Tick()
        {
            if (shotLeft <= 0) return;
            if (delay > 0)
            {
                delay--;
                return;
            }

            tickUntilNextShot--;
            if (tickUntilNextShot > 0) return;
            for (var i = 2; i > 0; i--)
            {
                DoShot(targetInfo);
            }

            shotLeft--;
            tickUntilNextShot = 5;
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (pawn.equipment?.Primary?.def?.IsRangedWeapon ?? false)
                return base.ValidateTarget(target, showMessages);
            Messages.Message("Main weapon must be ranged weapon", MessageTypeDefOf.NeutralEvent, false);
            return false;
        }

        public void DoShot(GlobalTargetInfo globalTargetInfo)
        {
            IntVec3 spawnPosition = VPERangerUtility.RandomCellAroundCellBase(globalTargetInfo.Cell, -3, 3);
            IntVec3 spawnPositionOffset = spawnPosition;
            spawnPositionOffset.z += 20;
            spawnPositionOffset.x -= 1;
            Projectile projectile =
                (Projectile)GenSpawn.Spawn(def.GetModExtension<AbilityExtension_Projectile>().projectile, spawnPosition,
                    pawn.Map);
            Pawn victim = spawnPosition.GetFirstPawn(pawn.Map);
            if (victim != null)
            {
                projectile.Launch(pawn, spawnPositionOffset.ToVector3Shifted(), victim, victim,
                    ProjectileHitFlags.IntendedTarget);
            }
            else
            {
                projectile.Launch(pawn, spawnPositionOffset.ToVector3Shifted(), spawnPosition, spawnPosition,
                    ProjectileHitFlags.All);
            }
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            Projectile projectile =
                (Projectile)GenSpawn.Spawn(def.GetModExtension<AbilityExtension_Projectile>().projectile, pawn.Position,
                    pawn.Map);
            projectile.Launch(pawn, new IntVec3(targets[0].Cell.x, targets[0].Cell.y, targets[0].Cell.z + 50),
                targets[0].Cell, ProjectileHitFlags.All);
            tickUntilNextShot = 5;
            shotLeft = 80;
            delay = 120;
            targetInfo = targets[0];
        }
    }
}
