using HarmonyLib;
using RimWorld;
using Verse;

namespace VPE_Ranger
{
    [HarmonyPatch(typeof(Bullet))]
    [HarmonyPatch("Impact")]
    public class Bullet_Impact_RicochetArrowPatch
    {
        private static void Postfix(Thing hitThing, Bullet __instance)
        {
            if (!(__instance?.Launcher is Pawn pawn) ||
                !pawn.health.hediffSet.HasHediff(VPERanger_DefOf.MakaiRanger_BuffTwo))
            {
                return;
            }

            HediffComp_RicochetArrow hediff = pawn.health.hediffSet
                .GetFirstHediffOfDef(VPERanger_DefOf.MakaiRanger_BuffTwo).TryGetComp<HediffComp_RicochetArrow>();
            if (hediff == null) return;
            int count = 0;
            if (hediff.ricochetCountLeft <= 0) return;
            foreach (Thing item in GenRadial.RadialDistinctThingsAround(__instance.Position, pawn.Map, 10f,
                         true))
            {
                if (!(item is Pawn pawnEn) ||
                    pawnEn == __instance.usedTarget ||
                    pawnEn.Downed || pawnEn.Dead || pawnEn.Faction == pawn.Faction ||
                    (!pawnEn.HostileTo(pawn) && !pawnEn.Faction.HostileTo(pawn.Faction))) continue;
                Projectile projectile = (Projectile)GenSpawn.Spawn(__instance.def, __instance.Position, pawn.Map);
                projectile.Launch(__instance.Launcher, item, item, ProjectileHitFlags.IntendedTarget);
                hediff.ricochetCountLeft -= 1;
                count++;
                if (count >= 1)
                {
                    break;
                }
            }

            if (hediff.ricochetCountLeft == 0)
            {
                pawn.health.RemoveHediff(
                    pawn.health.hediffSet.GetFirstHediffOfDef(VPERanger_DefOf.MakaiRanger_BuffTwo));
            }
        }
    }
}
